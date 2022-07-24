using System.Reflection;
using HBDStack.AzProxy.ServiceBus;
using HBDStack.AzProxy.ServiceBus.Internals;
using HBDStack.AzProxy.ServiceBus.Options;
using HBDStack.Framework.Extensions;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceBusSetup
{
    private static readonly Type InterfaceType = typeof(IBusMessageReceiver);

    private static IServiceCollection AddBusMessageReceiver<TImplementation>(this IServiceCollection services)
        where TImplementation : class, IBusMessageReceiver
    {
        if (services.Any(s => s.ImplementationType == typeof(TImplementation))) return services;

        return services
            .AddSingleton<IBusMessageReceiver, TImplementation>();
    }

    private static IServiceCollection AddBusMessageReceiver(this IServiceCollection services, Type implementationType)
    {
        if (services.Any(s => s.ImplementationType == implementationType)) return services;

        if (!implementationType.IsAssignableTo(InterfaceType))
            throw new ArgumentException($"{implementationType} is not assignable to {nameof(IBusMessageReceiver)}");

        return services
            .AddSingleton(InterfaceType, implementationType);
    }

    private static IServiceCollection ScanBusMessageReceivers(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies.Length == 0) return services;
        var types = assemblies.ScanClassesImplementOf(InterfaceType);

        foreach (var type in types)
            services.AddBusMessageReceiver(type);

        return services;
    }

    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration, params Assembly[] receiverAssemblies)
    {
        var config = new AzureServiceBusOptions();
        configuration.GetSection(AzureServiceBusOptions.Name).Bind(config);

        return services.AddServiceBus(config, receiverAssemblies);
    }

    /// <summary>
    /// Add all bus message service and scan receivers in the given assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="receiverAssemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddServiceBus(this IServiceCollection services, AzureServiceBusOptions configuration, params Assembly[] receiverAssemblies) =>
        services.ScanBusMessageReceivers(receiverAssemblies)
            .AddSingleton(Options.Options.Create(configuration))
            .AddSingleton(Options.Options.Create<IServiceBusOptions>(configuration))
            .AddSingleton<IAzureServiceBusFactory, AzureServiceBusFactory>()
            .AddSingleton<IBusMessageSenderFactory, BusMessageSenderFactory>()
            .AddSingleton<IBusMessageProcessorActivator, BusMessageProcessorActivator>();
}