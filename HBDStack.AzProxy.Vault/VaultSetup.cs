using HBDStack.AzProxy.Vault;
using HBDStack.AzProxy.Vault.Managers;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using HBDStack.AzProxy.Vault.Credentials;
using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Vault.Configs;

namespace Microsoft.Extensions.Configuration;

public static class VaultSetup
{
    #region Methods

    public static IServiceCollection AddVaultConfigures(this IServiceCollection services, IConfiguration configuration)
        => services.Configure<VaultConfig>(configuration.GetSection(VaultConfig.Name));

    /// <summary>
    /// Configure Vault. If credentials is not provided. It will use System Credentials
    /// https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity?tabs=dotnet#adding-a-system-assigned-identity
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="vaultInfo"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddVaults(this IConfigurationBuilder builder, VaultInfo vaultInfo, AppClientCredentials credential = null)
    {
        var manager = vaultInfo.Filter == null ? new DefaultKeyVaultSecretManager() : new KeyVaultSecretFilterManager(vaultInfo.Filter);
        var client = VaultClientCreator.Create(credential);

        return builder
            .AddAzureKeyVault(vaultInfo.VaultUri, client, manager);
    }

    /// <summary>
    /// Automatic load Vault info from Configuration. Ensure you have a section with <see cref="VaultConfig"/> in the configuration.
    /// Default looks up section name is VaultConfig. Provide sectionName if you wish to customize the name.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddVaults(this IConfigurationBuilder builder)
    {
        var config = builder.Build().GetConfig();

        if (config == null)
            return builder;

        return builder.AddVaults(config, config.CreateCredentials());
    }

    /// <summary>
    /// Add Vault Services [IMultiCertLoader]. Ensure you have a section with <see cref="VaultConfig"/> in the configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddVaultServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddVaultConfigures(configuration);

        return services
            .AddSingleton<IMultiCertLoader, MultiCertLoader>();
    }

    private static VaultConfig GetConfig(this IConfiguration configuration)
    {
        var config = new VaultConfig();
        configuration.Bind(VaultConfig.Name, config);

        if (!config.IsValid())
        {
            throw new InvalidOperationException($"Section {VaultConfig.Name} not found or invalid.");
        }

        Debug.WriteLine($"{nameof(VaultConfig)} found with name ${config.VaultName}");

        return config;
    }

    #endregion Methods
}