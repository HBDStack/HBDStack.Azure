using HBDStack.AzProxy.ServiceBus.Internals;

namespace HBDStack.AzProxy.ServiceBus;

public interface IBusMessageProcessorActivator : IAsyncDisposable
{
    Task StartProcessingAsync();
    Task StopProcessingAsync();
}

internal class BusMessageProcessorActivator : IBusMessageProcessorActivator
{
    private readonly IBusMessageReceiver[] receivers;
    private readonly IAzureServiceBusFactory busFactory;
    private List<InternalBusProcessor> processors = new();
    
    public BusMessageProcessorActivator(IEnumerable<IBusMessageReceiver> receivers, IAzureServiceBusFactory busFactory)
    {
        this.receivers = receivers.ToArray();
        this.busFactory = busFactory;

        if (!this.receivers.Any()) throw new ArgumentException($"No {nameof(IBusMessageReceiver)} found.");
    }

    public Task StartProcessingAsync()
    {
        processors = receivers.Select(r => busFactory.CreateProcessor(r)).ToList();
        return Task.WhenAll(processors.Select(p => p.StartProcessingAsync()));
    }

    public Task StopProcessingAsync()
    {
        if (!processors.Any()) return Task.CompletedTask;
        var tasks = processors.Select(p => p.StopProcessingAsync());
        return Task.WhenAll(tasks);
    }

    public async ValueTask DisposeAsync()
    {
        if (!processors.Any()) return;
        foreach (var processor in processors) await processor.DisposeAsync();
        foreach (var receiver in receivers) await receiver.DisposeAsync();
    }
}