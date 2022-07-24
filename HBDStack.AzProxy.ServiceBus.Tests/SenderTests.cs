using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HBDStack.AzProxy.ServiceBus.Tests;

public class SenderTests
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
            .AddServiceBus(config)
            .BuildServiceProvider();
    }

    [Test]
    public void GetFilterable_Properties()
    {
        var dic = new TestMessage("Hello") { Name = "Steven" }.GetFilterableProperties();
        dic[nameof(TestMessage.Name)].Should().Be("Steven");
    }
    
    [Test]
    public void GetFilterable_NullProperties()
    {
        var dic = new TestMessage("Hello").GetFilterableProperties();
        dic.Count.Should().Be(0);
    }

    [Test]
    public async Task Send_SingleMessage()
    {
        var sender = serviceProvider.GetRequiredService<IBusMessageSenderFactory>().CreateSender("drunkqueue");
        var func = () => sender.SendAsync(new TestMessage($"Hello World"));

        await func.Should().NotThrowAsync();
    }

    [Test]
    public async Task Send_BatchMessage()
    {
        var sender = serviceProvider.GetRequiredService<IBusMessageSenderFactory>().CreateSender("drunkqueue");

        var list = new List<TestMessage>();

        for (var i = 1; i <= 100; i++)
            list.Add(new TestMessage($"Hello World {i}"));

        var func = () => sender.SendBatchAsync(list);

        await func.Should().NotThrowAsync();
    }
}