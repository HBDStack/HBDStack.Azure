using System.Reflection;
using HBDStack.SlimMessageBus.AzureBus.Internals;
using Microsoft.Extensions.DependencyInjection;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.MsDependencyInjection;
using SlimMessageBus.Host.Serialization.SystemTextJson;

namespace HBDStack.SlimMessageBus.AzureBus;

public static class BusSetup
{
    public static IServiceCollection AddAzureBus(this IServiceCollection service, Action<IBusOptions> options, string connectionString,
        bool autoCleanupSubscriptionFilterRules = true,
        Assembly[]? addConsumersFromAssembly = null, Assembly[]? addConfiguratorsFromAssembly = null, Assembly[]? addInterceptorsFromAssembly = null)
    {
        var ops = new BusOptions();
        options(ops);

        service.AddSlimMessageBus((mbb, svp) =>
            {
                if (ops.Queues.Any())
                    foreach (var (_, queue) in ops.Queues)
                    {
                        if (queue.ProduceType is not null)
                            mbb.Produce(queue.ProduceType, b =>
                                b.DefaultQueue(queue.Name).WithBusMessageModifier());

                        if (queue.ConsumerHandlerType is not null)
                            mbb.Consume(queue.ConsumerMessageType, c =>
                                c.Instances(queue.Instances).WithConsumer(queue.ConsumerHandlerType));
                    }

                if (ops.Topics.Any())
                    foreach (var (_, tp) in ops.Topics)
                    {
                        if (tp.ProduceType is not null)
                            mbb.Produce(tp.ProduceType, b =>
                                b.DefaultTopic(tp.Name).WithBusMessageModifier());

                        if (tp.Subscriptions.Any())
                            foreach (var (_, sub) in tp.Subscriptions)
                            {
                                mbb.Consume(sub.ConsumerMessageType, c =>
                                {
                                    c.Topic(tp.Name).SubscriptionName(sub.Name)
                                        .WithConsumer(sub.ConsumerHandlerType)
                                        .Instances(sub.Instances);

                                    if (!string.IsNullOrWhiteSpace(sub.SqlFilter))
                                        c.SubscriptionSqlFilter(sub.SqlFilter, sub.SqlFilterName);
                                });
                            }
                    }

                //Azure Service Bus
                var settings = new ServiceBusMessageBusSettings(connectionString)
                {
                    TopologyProvisioning = new ServiceBusTopologySettings
                    {
                        Enabled = true,
                        // CanConsumerCreateQueue = false,
                        // CanConsumerCreateTopic = false,
                        // CanProducerCreateTopic = false,
                        // CanProducerCreateQueue = false,
                        CanConsumerCreateSubscription = true,
                        CreateSubscriptionOptions = op =>
                        {
                            op.EnableBatchedOperations = true;
                            op.MaxDeliveryCount = 10;
                            op.AutoDeleteOnIdle = TimeSpan.FromDays(60);
                            op.DeadLetteringOnMessageExpiration = true;
                            op.DefaultMessageTimeToLive = TimeSpan.FromDays(15);
                        }
                    }
                };

                if (autoCleanupSubscriptionFilterRules)
                    mbb.WithProvider(s => new ExtendedServiceBus(s, settings));
                else mbb.WithProviderServiceBus(settings);

                //Serializer
                mbb.WithSerializer(new JsonMessageSerializer());
            },
            addConsumersFromAssembly: addConsumersFromAssembly,
            addConfiguratorsFromAssembly: addConfiguratorsFromAssembly,
            addInterceptorsFromAssembly: addInterceptorsFromAssembly
        );
        return service;
    }
}