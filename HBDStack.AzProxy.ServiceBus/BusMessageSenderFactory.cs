using HBDStack.AzProxy.ServiceBus.Internals;
using HBDStack.AzProxy.ServiceBus.Options;
using Microsoft.Extensions.Options;

namespace HBDStack.AzProxy.ServiceBus;

public interface IBusMessageSenderFactory
{
    IBusMessageSender CreateSender(string name);
}

internal sealed class BusMessageSenderFactory : IBusMessageSenderFactory
{
    private readonly IAzureServiceBusFactory factory;
    private readonly IServiceBusOptions options;

    public BusMessageSenderFactory(IAzureServiceBusFactory factory, IOptions<IServiceBusOptions> options)
    {
        this.factory = factory;
        this.options = options.Value;
    }

    public IBusMessageSender CreateSender(string name)
    {
        var (sender, sessionEnabled) = factory.CreateSender(name);
        return new BusMessageSender(name, sessionEnabled, sender, options.JsonSerializerOptions);
    }
}