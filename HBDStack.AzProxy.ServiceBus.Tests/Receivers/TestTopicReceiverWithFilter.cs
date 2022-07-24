using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace HBDStack.AzProxy.ServiceBus.Tests.Receivers;

public class TestTopicReceiverWithFilter : IBusMessageReceiver, ISubscriptionSqlFilter
{
    public int Count { get; set; }

    public ValueTask DisposeAsync() => default;

    public (string name, string filter) GetSqlFilter() => ("filter-v2", "Name='HBD'");

    public (string topicOrQueueName, string? subcriptionName) Names => ("tp1", "sub2");

    public Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        Count++;
        Console.WriteLine("Topic {0} Received message: {1}", Names.subcriptionName, message.Body);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(ProcessErrorEventArgs args) => Task.CompletedTask;
}