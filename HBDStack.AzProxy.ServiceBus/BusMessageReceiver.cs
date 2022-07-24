using Azure.Messaging.ServiceBus;
using HBDStack.AzProxy.ServiceBus.Options;
using Microsoft.Extensions.Options;

namespace HBDStack.AzProxy.ServiceBus;

public interface ISubscriptionSqlFilter
{
    /// <summary>
    /// The custom filter for Subscription. The filter only be updated once the name is changed.
    /// </summary>
    public (string name, string filter) GetSqlFilter();
}

public interface IBusMessageReceiver : IAsyncDisposable
{
    /// <summary>
    /// The name of Topic or Queue
    /// The name of SubscriptionName or return null for Queue receiver.
    /// </summary>
    public (string topicOrQueueName, string? subcriptionName) Names { get; }

    public Task HandleMessageAsync(ServiceBusReceivedMessage message);
    public Task HandleErrorAsync(ProcessErrorEventArgs args);
}

public abstract class BusMessageReceiver<TMessage> : IBusMessageReceiver where TMessage : class
{
    private readonly IServiceBusOptions options;
    protected BusMessageReceiver(IOptions<IServiceBusOptions> options) => this.options = options.Value;

    public abstract (string topicOrQueueName, string? subcriptionName) Names { get; }

    public Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        var msg = message.ToBusMessage<TMessage>(options.JsonSerializerOptions);
        return HandleMessageAsync(msg);
    }

    protected abstract Task HandleMessageAsync(BusMessage<TMessage> message);

    public virtual Task HandleErrorAsync(ProcessErrorEventArgs args) => Task.CompletedTask;

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}