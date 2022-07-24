using System;
using System.Threading.Tasks;
using HBDStack.AzProxy.ServiceBus.Options;
using Microsoft.Extensions.Options;

namespace HBDStack.AzProxy.ServiceBus.Tests.Receivers;

public class TestTopicReceiver : BusMessageReceiver<TestMessage>
{
    public TestTopicReceiver(IOptions<AzureServiceBusOptions> options) : base(options)
    {
    }
    
    public static int Count { get; set; }
    
    public override (string topicOrQueueName, string? subcriptionName) Names => ("tp1", "sub1");

    protected override Task HandleMessageAsync(BusMessage<TestMessage> message)
    {
        Count++;
        Console.WriteLine("Topic {0} Received message: {1}", Names.subcriptionName, message.Body.Message);
        return Task.CompletedTask;
    }
}