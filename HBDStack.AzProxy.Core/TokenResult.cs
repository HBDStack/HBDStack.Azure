namespace HBDStack.AzProxy.Core.AuthContexts;

public sealed class TokenResult
{
    #region Constructors

    public TokenResult(string tenantId, string accessToken, string idToken = null)
    {
        TenantId = tenantId;
        AccessToken = accessToken;
        IdToken = idToken;
    }

    #endregion Constructors

    #region Properties

    public string AccessToken { get; }
    public string IdToken { get; }
    public string TenantId { get; }

    #endregion Properties
}