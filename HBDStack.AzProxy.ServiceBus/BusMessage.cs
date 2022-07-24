using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;

namespace HBDStack.AzProxy.ServiceBus;

public sealed class BusMessage<TMessage> : ServiceBusMessage where TMessage : class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions DefaultOptions = new()
        { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase,DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull};

    public BusMessage(TMessage messageBody, JsonSerializerOptions? options = null)
        : base(JsonSerializer.Serialize(messageBody, options ?? DefaultOptions))
    {
        Body = messageBody;

        var filterableProperties = messageBody.GetFilterableProperties();
        foreach (var property in filterableProperties)
        {
            if (ApplicationProperties.ContainsKey(property.Key)) continue;
            ApplicationProperties.Add(property.Key, property.Value);
        }
    }

    public BusMessage(ServiceBusReceivedMessage receivedMessage, JsonSerializerOptions? options = null) : base(
        receivedMessage)
        => Body = JsonSerializer.Deserialize<TMessage>(receivedMessage.Body, options ?? DefaultOptions)!;

    public new TMessage Body { get; }
}