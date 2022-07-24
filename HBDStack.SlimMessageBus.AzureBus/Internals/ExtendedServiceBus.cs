using Microsoft.Extensions.Logging;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Config;

namespace HBDStack.SlimMessageBus.AzureBus.Internals;

internal class ExtendedServiceBus : ServiceBusMessageBus
{
    public ExtendedServiceBus(MessageBusSettings settings, ServiceBusMessageBusSettings providerSettings) : base(settings, providerSettings)
    {
    }
    
    protected override async Task OnStart()
    {
        await UpdateSubFilterAsync();
        await base.OnStart();
    }

    /// <summary>
    /// Update the Subscriptions Filter here
    /// </summary>
    /// <returns></returns>
    private async Task UpdateSubFilterAsync()
    {
        var logger = LoggerFactory.CreateLogger<ExtendedServiceBus>();
        try
        {
            var adminClient = ProviderSettings.AdminClientFactory();

            async Task CleanUpOtherRules(
                string path,
                string subscriptionName,
                string ruleName)
            {
                //Delete all existing Rules
                await foreach (var r in adminClient.GetRulesAsync(path, subscriptionName))
                {
                    if(r.Name.Equals(ruleName,StringComparison.CurrentCultureIgnoreCase))continue;
                    await adminClient.DeleteRuleAsync(path, subscriptionName, r.Name);
                }
            }

            var consumersSettingsByPath = Settings.Consumers.OfType<AbstractConsumerSettings?>()
                .Where(x=>x is not null && x.PathKind == PathKind.Topic)
                .GroupBy(x => x!.Path)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var (path, consumerSettingsList) in consumersSettingsByPath)
            {
                var consumerSettingsBySubscription = consumerSettingsList
                    .Select(x => (ConsumerSettings: x, SubscriptionName: x!.GetSubscriptionName(required: false)))
                    .Where(x => x.SubscriptionName != null)
                    .ToDictionary(x => x.SubscriptionName!, x => x.ConsumerSettings);

                foreach (var (subscriptionName, consumerSettings) in consumerSettingsBySubscription)
                {
                    var filters = consumerSettings!.GetRules()?.Values;
                    if (filters is not { Count: > 0 }) continue;

                    //Check Subscription Exists
                    if (!await adminClient.SubscriptionExistsAsync(path, subscriptionName)) 
                        continue;
                    
                    await Task.WhenAll(filters.Select(filter => CleanUpOtherRules(path, subscriptionName!, filter.Name)));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not update Subscription Sql filter.");
        }
    }
}