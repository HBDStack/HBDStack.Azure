using HBDStack.AzProxy.Core.Providers;
using HBDStack.AzProxy.Vault.Configs;
using HBDStack.AzProxy.Vault.Providers;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    #region Methods

    public static ICertProvider GetVaultCertProvider(this IServiceProvider provider, string certName)
    {
        var config = provider.GetService<IOptions<VaultConfig>>();
        return new VaultCertProvider(certName, config);
    }

    #endregion Methods
}