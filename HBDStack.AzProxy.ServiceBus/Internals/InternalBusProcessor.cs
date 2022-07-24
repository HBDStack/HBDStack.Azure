using Azure.Messaging.ServiceBus.Administration;
using HBDStack.AzProxy.ServiceBus.Options;

namespace HBDStack.AzProxy.ServiceBus.Internals;

internal sealed class InternalBusProcessor : IAsyncDisposable
{
    private readonly dynamic azureBusProcessor;
    private readonly IBusMessageReceiver receiver;
    private readonly AzureServiceBusConfig config;

    public InternalBusProcessor(dynamic azureBusProcessor, IBusMessageReceiver receiver, AzureServiceBusConfig config)
    {
        this.azureBusProcessor = azureBusProcessor ?? throw new ArgumentNullException(nameof(azureBusProcessor));
        this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        this.config = config;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        await TryUpdateSqlFilterAsync(cancellationToken);
        await azureBusProcessor.StartProcessingAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task StopProcessingAsync(CancellationToken cancellationToken = default)
        => await azureBusProcessor.StopProcessingAsync(cancellationToken).ConfigureAwait(false);

    private async Task TryUpdateSqlFilterAsync(CancellationToken cancellationToken = default)
    {
        if (receiver is not ISubscriptionSqlFilter sqlFilter ||
            string.IsNullOrWhiteSpace(receiver.Names.subcriptionName)) return;

        var client = new ServiceBusAdministrationClient(config.ConnectionString);
        var filter = sqlFilter.GetSqlFilter();

        //1. Remove current rules
        var rules = await client
            .GetRulesAsync(receiver.Names.topicOrQueueName, receiver.Names.subcriptionName, cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);

        //if the name is existed then do NOTHING
        if (rules.Any(r => r.Name == filter.name)) return;

        //Delete old rules
        foreach (var rule in rules)
            await client.DeleteRuleAsync(receiver.Names.topicOrQueueName, receiver.Names.subcriptionName, rule.Name,
                cancellationToken);
        
        //2. Update with new filter
        await client.CreateRuleAsync(receiver.Names.topicOrQueueName, receiver.Names.subcriptionName,
            new CreateRuleOptions
            {
                Name = filter.name,
                Filter = new SqlRuleFilter(filter.filter),
                //Action = new SqlRuleAction("SET quantity = quantity / 2; REMOVE priority;SET sys.CorrelationId = 'low';")
            }, cancellationToken);
    }

    public ValueTask DisposeAsync() => azureBusProcessor?.DisposeAsync();
}