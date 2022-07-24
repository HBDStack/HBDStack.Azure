using Microsoft.Identity.Client;
using HBDStack.AzProxy.Core.AuthContexts;

namespace HBDStack.AzProxy.Core.AzAD;

public class AuthContext
{
    #region Fields

    private readonly AppCredentials credentials;
    private string[] DefaultScopes => credentials.Scope ?? new[] { $"{credentials.ClientId}/.default" };

    #endregion Fields

    #region Constructors

    public AuthContext(AppCredentials credentials) => this.credentials = credentials;

    #endregion Constructors

    #region Methods

    private void Validate()
    {
        if (credentials.UseSystemCredentials)
            throw new InvalidOperationException(nameof(credentials.UseSystemCredentials));
    }

    private async ValueTask<ConfidentialClientApplicationBuilder> CreateBuilderAsync()
    {
        var builder = ConfidentialClientApplicationBuilder
            .Create(credentials.ClientId)
            .WithAuthority(credentials.Authority);

        if (credentials.CertProvider != null)
        {
            var cert = await credentials.CertProvider.GetCertAsync();

            builder.WithCertificate(cert);
        }
        else if (!string.IsNullOrEmpty(credentials.ClientId))
        {
            builder.WithClientSecret(credentials.ClientSecret);
        }
        return builder;
    }

    private TokenResult CreateResult(AuthenticationResult result)
        => new(result.TenantId ?? credentials.Authority, result.AccessToken, result.IdToken);

    private PublicClientApplicationBuilder CreatePublicBuilder()
    {
        var builder = PublicClientApplicationBuilder
            .Create(credentials.ClientId)
            .WithAuthority(credentials.Authority);

        return builder;
    }

    /// <summary>
    /// Login on behaft of users
    /// </summary>
    /// <param name="userCredentials">The "Treat application as a public client" need to be enabled and support "oauth2AllowIdTokenImplicitFlow".</param>
    /// <param name="resource"></param>
    /// <returns></returns>
    public virtual async Task<TokenResult> AcquireTokenOnBehalfOfAsync(string idToken, string[] scope = null)
    {
        Validate();
        //Code reviewer: https://login.microsoftonline.com/error?code=700021
        var scopes = scope ?? DefaultScopes;

        var auth = (await CreateBuilderAsync()).Build();
        var user = new UserAssertion(idToken);
        var result = await auth.AcquireTokenOnBehalfOf(scopes, user).ExecuteAsync();

        return CreateResult(result);
    }

    public virtual async Task<TokenResult> AcquireTokenByUsernamePassword(UserCredentials userCredentials, string[] scope = null)
    {
        Validate();

        var scopes = scope ?? DefaultScopes;
        var app = CreatePublicBuilder().Build();
        var accounts = await app.GetAccountsAsync();
        var ac = accounts.FirstOrDefault(a => a.Username == userCredentials.UserName);

        AuthenticationResult result;

        if (ac != null)
            result = await app.AcquireTokenSilent(scopes, ac).ExecuteAsync();
        else result = await app.AcquireTokenByUsernamePassword(scopes, userCredentials.UserName, userCredentials.Password).ExecuteAsync();

        return CreateResult(result);
    }

    public virtual async Task<TokenResult> AcquireTokenAsync(string[] scope = null)
    {
        Validate();

        var scopes = scope ?? DefaultScopes;
        var auth = (await CreateBuilderAsync()).Build();

        var result = await auth.AcquireTokenForClient(scopes).ExecuteAsync();
        return CreateResult(result);
    }

    #endregion Methods
}