namespace HBDStack.SlimMessageBus.AzureBus;

public interface IMessage
{
    public string? ReplyToSessionId { get; set; }
    public string? ReplyTo { get; set; }
    public string? SessionId { get; set; }
}