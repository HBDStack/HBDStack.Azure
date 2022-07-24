using HBDStack.AzProxy.ServiceBus.Attributes;

namespace HBDStack.AzProxy.ServiceBus.Tests;

public class TestMessage
{
    public TestMessage(string message) => Message = message;
    public string Message { get; set; }
    
    [BusFilterable]
    public string? Name { get; set; }
}