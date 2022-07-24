using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Core.Providers;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using HBDStack.AzProxy.Vault.Configs;
using HBDStack.AzProxy.Vault.Credentials;
using HBDStack.AzProxy.Vault.Providers;

namespace HBDStack.AzProxy.Vault;

public class MultiCertLoader : IMultiCertLoader
{
    #region Fields

    private readonly ConcurrentDictionary<string, ICertProvider> loaders;
    private readonly AppClientCredentials credential;
    private readonly VaultInfo vaultInfo;

    #endregion Fields

    #region Constructors

    public MultiCertLoader(IOptions<VaultConfig> vaultConfig)
        : this(vaultConfig.Value, vaultConfig.Value.CreateCredentials())
    {
    }

    /// <summary>
    /// Create VaultCertLoader with System Identity Credential
    /// https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity?tabs=dotnet#adding-a-system-assigned-identity
    /// </summary>
    /// <param name="certName"></param>
    /// <param name="vaultInfo"></param>
    /// <param name="credential"></param>
    public MultiCertLoader(VaultInfo vaultInfo)
        : this(vaultInfo, null)
    {
    }

    public MultiCertLoader(VaultInfo vaultInfo, AppClientCredentials credential)
    {
        this.vaultInfo = vaultInfo ?? throw new ArgumentNullException(nameof(vaultInfo));
        this.credential = credential;
        loaders = new ConcurrentDictionary<string, ICertProvider>();
    }

    #endregion Constructors

    #region Methods

    public ValueTask<X509Certificate2> GetCertAsync(string certName)
    {
        if (string.IsNullOrEmpty(certName))
            throw new ArgumentException("message", nameof(certName));

        var loader = GetOrCreate(certName);
        return loader.GetCertAsync();
    }

    protected ICertProvider GetOrCreate(string name) => loaders.GetOrAdd(name.ToLower(), n => new VaultCertProvider(name, vaultInfo, credential));

    #endregion Methods
}