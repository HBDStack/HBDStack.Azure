using HBDStack.SlimMessageBus.AzureBus.Tests.Data;
using Microsoft.Extensions.DependencyInjection;
using SlimMessageBus;
using SlimMessageBus.Host;

namespace HBDStack.SlimMessageBus.AzureBus.Tests;

public class BusTests : IClassFixture<BusFixture>
{
    private readonly BusFixture fixture;

    public BusTests(BusFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task CreateFilter()
    {
        fixture.Provider.GetRequiredService<IMasterMessageBus>();
        
        var bus = fixture.Provider.GetRequiredService<IMessageBus>();
        await bus.Publish(new Message { Body = "Hello Steven"});

        await Task.Delay(TimeSpan.FromSeconds(15));
    }
}