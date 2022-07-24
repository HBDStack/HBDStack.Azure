using System.Text.Json;

namespace HBDStack.AzProxy.ServiceBus.Options;

public class AzureServiceBusConfig
{
    public string ConnectionString { get; set; } = default!;
    internal string Name { get; set; } = default!;
    public bool SessionEnabled { get; set; } = default!;
    
    public BusProcessorOptions? Options { get; set; }
}

public class AzureServiceBusOptions : Dictionary<string, AzureServiceBusConfig>,IServiceBusOptions
{
    public static string Name => "AzureServiceBus";
    
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new(JsonSerializerDefaults.Web);
 
}