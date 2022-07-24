using System.Text.Json;

namespace HBDStack.AzProxy.ServiceBus.Options;

public interface IServiceBusOptions
{
    JsonSerializerOptions JsonSerializerOptions { get; set; }
}