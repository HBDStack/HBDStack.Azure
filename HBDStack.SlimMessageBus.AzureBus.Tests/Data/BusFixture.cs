using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HBDStack.SlimMessageBus.AzureBus.Tests.Data;

public class BusFixture : IDisposable
{
    public BusFixture()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        Provider = new ServiceCollection()
            .AddLogging(l=>l.AddConsole())
            .AddAzureBus(b =>
                {
                    b.Topic("topic-1")
                        .Produce<Message>()
                        .Subscription("sub-4")
                        .SubscriptionSqlFilter("Hello=2", "filter-v1")
                        .Consume<ConsumerHandler>();
                    
                    b.Topic("topic-1")
                        .Subscription("sub-1")
                        .Consume<Message, ConsumerHandler>();
                }, config.GetConnectionString("AzureBus"),
                autoCleanupSubscriptionFilterRules: true,
                addConsumersFromAssembly:new Assembly[]{typeof(BusFixture).Assembly})
            .BuildServiceProvider();
    }

    public ServiceProvider Provider { get; set; }

    public void Dispose() => Provider?.Dispose();
}