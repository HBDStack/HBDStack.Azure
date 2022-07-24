using HBDStack.SlimMessageBus.AzureBus.Tests.Data;

namespace HBDStack.SlimMessageBus.AzureBus.Tests;

public class OptionsTests
{
    [Fact]
    public void Topics()
    {
        var option = new BusOptions();
        option.Topic("tp-1")
            .Produce<Message>()
            .Subscription("sub-1").SubscriptionSqlFilter("Hellow=1")
                .Consume<ConsumerHandler>();
    }
    
    [Fact]
    public void Queues()
    {
        var option = new BusOptions();
        option.Queue("qu-1")
            .Produce<Message>()
            .Consume<ConsumerHandler>();
    }
}