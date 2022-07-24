using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Core.Providers;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using HBDStack.AzProxy.Vault.Configs;
using HBDStack.AzProxy.Vault.Credentials;

namespace HBDStack.AzProxy.Vault.Providers;

public class VaultCertProvider : CertProvider
{
    #region Fields

    private readonly string certName;
    private readonly AppClientCredentials credential;
    private readonly VaultInfo vaultInfo;

    #endregion Fields

    #region Constructors

    public VaultCertProvider(string certName, IOptions<VaultConfig> vaultConfig)
        : this(certName, vaultConfig.Value, vaultConfig.Value.CreateCredentials())
    {
    }

    /// <summary>
    /// Create VaultCertLoader with System Identity Credential
    /// https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity?tabs=dotnet#adding-a-system-assigned-identity
    /// </summary>
    /// <param name="certName"></param>
    /// <param name="vaultInfo"></param>
    /// <param name="credential"></param>
    public VaultCertProvider(string certName, VaultInfo vaultInfo)
        : this(certName, vaultInfo, null)
    {
    }

    public VaultCertProvider(string certName, VaultInfo vaultInfo, AppClientCredentials credential)
    {
        if (string.IsNullOrEmpty(certName))
            throw new ArgumentException("message", nameof(certName));

        this.vaultInfo = vaultInfo ?? throw new ArgumentNullException(nameof(vaultInfo));
        this.credential = credential;
        this.certName = certName;
    }

    #endregion Constructors

    #region Methods

    protected override async Task<X509Certificate2> InternalLoadCertAsync(CancellationToken cancellationToken)
    {
        using var client = VaultClientCreator.Create(credential);
        var certificateSecret = await client.GetSecretAsync(vaultInfo.VaultUri, certName);
        var privateKeyBytes = Convert.FromBase64String(certificateSecret.Value);

        cancellationToken.ThrowIfCancellationRequested();

        return new X509Certificate2(privateKeyBytes, (string)null);
    }

    #endregion Methods
}