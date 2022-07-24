namespace HBDStack.AzProxy.ServiceBus.Options;

public class BusProcessorOptions
{
    public int PrefetchCount { get; set; } = 10;
    public int MaxConcurrentCalls { get; set; } = 5;
    public int MaxConcurrentCallsPerSession { get; set; } = 5;
    public IList<string> SessionIds { get; } = new List<string>();
}