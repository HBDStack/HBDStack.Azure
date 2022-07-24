using System.Collections.Concurrent;
using Azure.Messaging.ServiceBus;
using HBDStack.AzProxy.ServiceBus.Options;
using Microsoft.Extensions.Options;

namespace HBDStack.AzProxy.ServiceBus.Internals;

internal interface IAzureServiceBusFactory
{
    (ServiceBusSender sender, bool sessionEnabled) CreateSender(string name);
    InternalBusProcessor CreateProcessor(IBusMessageReceiver receiver);
}

internal sealed class AzureServiceBusFactory : IAzureServiceBusFactory, IAsyncDisposable
{
    private readonly AzureServiceBusOptions option;
    private readonly ConcurrentDictionary<string, ServiceBusClient> clientCache = new();
    private readonly ConcurrentDictionary<string, ServiceBusSender> cache = new();

    public AzureServiceBusFactory(IOptions<AzureServiceBusOptions> option) => this.option = option.Value;

    private AzureServiceBusConfig TryGetConfig(string name)
    {
        option.TryGetValue(name, out var config);
        if (config == null) throw new ArgumentException($"The name {name} is not found in the configuration.");

        if (string.IsNullOrEmpty(config.Name))
            config.Name = name;
        
        return config;
    }

    public (ServiceBusSender sender, bool sessionEnabled) CreateSender(string name)
    {
        var config = TryGetConfig(name);

        var instance = cache.GetOrAdd(name, key =>
        {
            var client = clientCache.GetOrAdd(config.ConnectionString, k => new ServiceBusClient(k));
            var sender = client.CreateSender(config.Name);
            return sender;
        });

        return (instance, config.SessionEnabled);
    }

    public InternalBusProcessor CreateProcessor(IBusMessageReceiver receiver)
    {
        if (receiver == null) throw new ArgumentNullException(nameof(receiver));

        var config = TryGetConfig(receiver.Names.topicOrQueueName);
        var options = config.Options ?? new BusProcessorOptions();

        var client = clientCache.GetOrAdd(config.ConnectionString, k => new ServiceBusClient(k));
        InternalBusProcessor finalProcessor;

        if (config.SessionEnabled)
        {
            var op = new ServiceBusSessionProcessorOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = true,
                PrefetchCount = options.PrefetchCount,
                MaxConcurrentSessions = options.MaxConcurrentCalls,
                MaxConcurrentCallsPerSession = options.MaxConcurrentCallsPerSession
            };

            if (options.SessionIds.Any())
                op.SessionIds.AddRange(options.SessionIds);

            var processor = string.IsNullOrEmpty(receiver.Names.subcriptionName)
                ? client.CreateSessionProcessor(config.Name, op)
                : client.CreateSessionProcessor(config.Name, receiver.Names.subcriptionName, op);

            processor.ProcessMessageAsync += (args) => receiver.HandleMessageAsync(args.Message);
            processor.ProcessErrorAsync += receiver.HandleErrorAsync;

            finalProcessor = new InternalBusProcessor(processor, receiver, config);
        }
        else
        {
            var op = new ServiceBusProcessorOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = true,
                PrefetchCount = options.PrefetchCount,
                MaxConcurrentCalls = options.MaxConcurrentCalls,
            };

            var processor = string.IsNullOrEmpty(receiver.Names.subcriptionName)
                ? client.CreateProcessor(config.Name, op)
                : client.CreateProcessor(config.Name, receiver.Names.subcriptionName, op);

            processor.ProcessMessageAsync += (args) => receiver.HandleMessageAsync(args.Message);
            processor.ProcessErrorAsync += receiver.HandleErrorAsync;

            finalProcessor = new InternalBusProcessor(processor, receiver, config);
        }

        return finalProcessor;
    }
    
    public async ValueTask DisposeAsync()
    {
        foreach (var client in clientCache.Values) await client.DisposeAsync();
        foreach (var sender in cache.Values) await sender.DisposeAsync();
    }
}