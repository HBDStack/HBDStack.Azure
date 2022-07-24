using HBDStack.AzProxy.Core.AzAD;

namespace HBDStack.AzProxy.Core.Providers;

public class AzAdTokenProvider : TokenProvider
{
    #region Fields

    private readonly AuthContext context;

    #endregion Fields

    #region Constructors

    public AzAdTokenProvider(AppCredentials credential) => context = new AuthContext(credential);

    #endregion Constructors

    #region Methods

    protected override async ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var token = await context.AcquireTokenAsync();
        return token?.AccessToken;
    }

    #endregion Methods
}