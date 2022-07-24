using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Vault.Configs;
using HBDStack.AzProxy.Vault.Providers;
using Microsoft.Extensions.Options;

namespace HBDStack.AzProxy.Vault;

public static class HttpClientBuilderExtensions
{
    #region Methods

    public static IHttpClientBuilder ClientCertificate(this IHttpClientBuilder builder, string certName, IOptions<VaultConfig> config)
        => builder.ClientCertificate(new VaultCertProvider(certName, config));

    #endregion Methods
}