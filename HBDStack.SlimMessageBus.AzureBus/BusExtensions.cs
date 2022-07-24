using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Config;

namespace HBDStack.SlimMessageBus.AzureBus;

public static class BusExtensions
{
    public static ProducerBuilder<T> WithBusMessageModifier<T>(this ProducerBuilder<T> producerBuilder) =>
        producerBuilder.WithModifier((message, busMessage) =>
        {
            var type = message!.GetType();

            if (message is not IMessage msg) return;

            busMessage.SessionId = msg.SessionId;
            busMessage.ReplyTo = msg.ReplyTo;
            busMessage.ReplyToSessionId = msg.ReplyToSessionId;

            if (type.GetCustomAttributes(typeof(BusMessagePropertiesAttribute), true).FirstOrDefault() is not BusMessagePropertiesAttribute att) return;
            foreach (var p in att.Properties)
            {
                var value = type.GetProperty(p)!.GetValue(message);
                if (value is null || value is string v && string.IsNullOrWhiteSpace(v)) continue;

                busMessage.ApplicationProperties.Add(p, value);
            }
        });
    
    internal static string? GetSubscriptionName(
        this AbstractConsumerSettings consumerSettings,
        bool required = true) =>
        !consumerSettings.Properties.ContainsKey("SubscriptionName") && !required ? null : consumerSettings.Properties["SubscriptionName"] as string;
    
    internal static IDictionary<string, SubscriptionSqlRule>? GetRules(
        this AbstractConsumerSettings consumerSettings,
        bool createIfNotExists = false)
    {
        var rules = consumerSettings.GetOrDefault<IDictionary<string, SubscriptionSqlRule>>("Asb_Rules");
        if (rules == null & createIfNotExists)
        {
            rules = new Dictionary<string, SubscriptionSqlRule>();
            consumerSettings.Properties["Asb_Rules"] = (object) rules;
        }
        return rules;
    }
}