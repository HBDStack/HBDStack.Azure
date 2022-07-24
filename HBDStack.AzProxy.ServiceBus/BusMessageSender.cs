using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace HBDStack.AzProxy.ServiceBus;

public interface IBusMessageSender
{
    public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;

    /// <summary>
    /// Send a single message to the queue or topic.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public Task SendAsync<TMessage>(BusMessage<TMessage> message, CancellationToken cancellationToken = default) where TMessage : class;

    public Task SendBatchAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class;

    /// <summary>
    /// Send a batch of messages to the queue or topic.
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public Task SendBatchAsync<TMessage>(IEnumerable<BusMessage<TMessage>> messages, CancellationToken cancellationToken = default) where TMessage : class;
}

internal class BusMessageSender : IBusMessageSender
{
    private readonly ServiceBusSender sender;
    private readonly JsonSerializerOptions options;
    public string Name { get; }
    public bool SessionEnabled { get; }

    public BusMessageSender(string name, bool sessionEnabled, ServiceBusSender sender, JsonSerializerOptions options)
    {
        this.sender = sender;
        this.options = options;
        Name = name;
        SessionEnabled = sessionEnabled;
    }

    public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        => SendAsync(new BusMessage<TMessage>(message, options), cancellationToken);

    public Task SendAsync<TMessage>(BusMessage<TMessage> message, CancellationToken cancellationToken = default)
        where TMessage : class =>
        sender.SendMessageAsync(message, cancellationToken);

    public Task SendBatchAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : class
        => SendBatchAsync(messages.Select(m=> new BusMessage<TMessage>(m, options)), cancellationToken);

    public async Task SendBatchAsync<TMessage>(IEnumerable<BusMessage<TMessage>> messages, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var batch = await sender.CreateMessageBatchAsync(cancellationToken);

        foreach (var item in messages)
            if (!batch.TryAddMessage(item))
                throw new OperationCanceledException($"The message {item.MessageId} is too large to fit in the batch.");

        await sender.SendMessagesAsync(batch, cancellationToken);
    }
}