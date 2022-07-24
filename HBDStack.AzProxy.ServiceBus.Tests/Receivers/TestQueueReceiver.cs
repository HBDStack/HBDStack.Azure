using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace HBDStack.AzProxy.ServiceBus.Tests.Receivers;

public class TestQueueReceiver : IBusMessageReceiver
{
    public int Count { get; set; }

    public ValueTask DisposeAsync() => default;
    
    public (string topicOrQueueName, string? subcriptionName) Names => ("drunkqueue", null);

    public Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        Count++;
        Console.WriteLine("Queue {0} Received message: {1}", Names.topicOrQueueName, message.Body.ToString());
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(ProcessErrorEventArgs args) => Task.CompletedTask;
}