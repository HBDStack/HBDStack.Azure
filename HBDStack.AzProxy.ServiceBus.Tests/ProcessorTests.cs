using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HBDStack.AzProxy.ServiceBus.Tests.Receivers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HBDStack.AzProxy.ServiceBus.Tests;

public class ProcessorTests
{
    private IServiceProvider serviceProvider = null!;

    [SetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddServiceBus(config, typeof(SenderTests).Assembly)
            .BuildServiceProvider();
    }

    [Test]
    [Order(1)]
    public async Task SendAndReceiveQueueMessage()
    {
        var receivers = serviceProvider.GetServices<IBusMessageReceiver>();
        var activator = serviceProvider.GetRequiredService<IBusMessageProcessorActivator>();
        await activator.StartProcessingAsync();

        var sender = serviceProvider.GetRequiredService<IBusMessageSenderFactory>().CreateSender("drunkqueue");
        await sender.SendAsync(new TestMessage($"Hello World"));

        await Task.Delay(TimeSpan.FromSeconds(2));
        var receiver = receivers.OfType<TestQueueReceiver>().First();

        receiver.Count.Should().BeGreaterOrEqualTo(1);
        Console.WriteLine($"Received {0} message from Queue", receiver.Count);

        await activator.StopProcessingAsync();
    }

    [Test]
    [Order(2)]
    public async Task SendAndReceiveTopicMessage()
    {
        TestTopicReceiver.Count = 0;
        var receivers = serviceProvider.GetServices<IBusMessageReceiver>();
        var activator = serviceProvider.GetRequiredService<IBusMessageProcessorActivator>();

        await activator.StartProcessingAsync();

        var sender = serviceProvider.GetRequiredService<IBusMessageSenderFactory>().CreateSender("tp1");
        await sender.SendAsync(new TestMessage($"Hello World"));

        await Task.Delay(TimeSpan.FromSeconds(30));
        var receiver = receivers.OfType<TestTopicReceiver>().First();

        TestTopicReceiver.Count.Should().BeGreaterOrEqualTo(1);
        Console.WriteLine("Received {0} message from Topic", TestTopicReceiver.Count);

        await activator.StopProcessingAsync();
    }

    [Test]
    [Order(2)]
    public async Task SendAndReceiveTopicWithFilterMessage()
    {
        var receivers = serviceProvider.GetServices<IBusMessageReceiver>();
        var activator = serviceProvider.GetRequiredService<IBusMessageProcessorActivator>();

        await activator.StartProcessingAsync();

        var sender = serviceProvider.GetRequiredService<IBusMessageSenderFactory>().CreateSender("tp1");
        await sender.SendAsync(new BusMessage<TestMessage>(new TestMessage($"Hello World") { Name = "HBD" })
            { ApplicationProperties = { { "OtherProperty", "HBD" } } });

        await Task.Delay(TimeSpan.FromSeconds(30));
        var receiver = receivers.OfType<TestTopicReceiverWithFilter>().First();

        receiver.Count.Should().BeGreaterOrEqualTo(1);
        Console.WriteLine("Received {0} message from Topic", receiver.Count);

        await activator.StopProcessingAsync();
    }
}