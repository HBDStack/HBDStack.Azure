using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Core.AzAD;
using HBDStack.AzProxy.Core.Providers;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System.Diagnostics.CodeAnalysis;

namespace HBDStack.AzProxy.Vault;

public static class VaultClientCreator
{
    #region Methods

    public static KeyVaultClient Create(AppClientCredentials credential)
    {
        if (credential == null || credential.UseSystemCredentials)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        return new KeyVaultClient(async (authority, resource, scope) =>
        {
            var credentials = new AppCredentials(authority, credential);
            credentials.Scope = new[] { scope};
            var authContext = new AuthContext(credentials);
                
            var token = (await authContext.AcquireTokenAsync(new[] { $"{resource}/.default" }))?.AccessToken;
            return token;
        });
    }

    /// <summary>
    /// Create KeyVaultClient from ClientId and Secret. If ClientId is not provided the SystemCredentials will be used
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    public static KeyVaultClient Create([AllowNull] string clientId, [AllowNull] string clientSecret) => Create(new AppClientCredentials(clientId, clientSecret));

    /// <summary>
    /// Create KeyVaultClient from ClientId and Certificate. If ClientId is not provided the SystemCredentials will be used
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    public static KeyVaultClient Create([AllowNull] string clientId, [AllowNull] ICertProvider cert) => Create(new AppClientCredentials(clientId, cert));

    /// <summary>
    /// Create KeyVaultClient using SystemCredentials
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    public static KeyVaultClient Create() => Create(new AppClientCredentials(null));

    #endregion Methods
}