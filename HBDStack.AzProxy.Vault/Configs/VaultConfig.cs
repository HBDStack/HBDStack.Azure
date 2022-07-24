using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Core.Providers;
using HBDStack.AzProxy.Vault.Credentials;

namespace HBDStack.AzProxy.Vault.Configs;

/// <summary>
/// This is use to load the configuration from appSettings.json
/// </summary>
public class VaultConfig : VaultInfo
{
    #region Properties

    /// <summary>
    /// The section name in appSettings.json
    /// </summary>
    public static string Name => nameof(VaultConfig);

    public string CertPassword { get; set; }

    public string ClientCertPath { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    #endregion Properties

    #region Methods

    public AppClientCredentials CreateCredentials()
        => string.IsNullOrEmpty(ClientSecret)
            ? new AppClientCredentials(ClientId, new CertFileProvider(ClientCertPath, CertPassword))
            : new AppClientCredentials(ClientId, ClientSecret);

    internal bool IsValid() => !string.IsNullOrEmpty(VaultName);

    #endregion Methods
}