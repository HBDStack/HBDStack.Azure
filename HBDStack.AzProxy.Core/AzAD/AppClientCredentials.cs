using HBDStack.AzProxy.Core.Providers;

namespace HBDStack.AzProxy.Core;

/// <summary>
/// The Azure Client Credentials
/// </summary>
public class AppClientCredentials
{
    #region Constructors

    public AppClientCredentials(string clientId, string secret)
    {
        ClientId = clientId;
        ClientSecret = secret;
    }

    public AppClientCredentials(string clientId, ICertProvider certProvider)
    {
        ClientId = clientId;
        CertProvider = certProvider;
    }

    /// <summary>
    /// Use Managed System Credentials
    /// </summary>
    public AppClientCredentials(AppClientCredentials credentials)
    {
        CertProvider = credentials.CertProvider;
        ClientId = credentials.ClientId;
        ClientSecret = credentials.ClientSecret;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Connect to Azure Resource using certificate as Secret.
    /// </summary>
    public ICertProvider CertProvider { get; }

    public string ClientId { get; }

    /// <summary>
    /// Connect to Azure Resource using ClientSecret
    /// </summary>
    public string ClientSecret { get; }

    /// <summary>
    /// Identityer whether should use System Assign account or using clientId and Secret.
    /// If Client is null or empty this flag will be true.
    /// </summary>
    public bool UseSystemCredentials => string.IsNullOrEmpty(ClientId);

    #endregion Properties
}