namespace HBDStack.AzProxy.Core.Providers;

public sealed class StaticAuthTokenProvider : TokenProvider
{
    #region Fields

    private readonly string token;

    #endregion Fields

    #region Constructors

    public StaticAuthTokenProvider(string token) => this.token = token;

    #endregion Constructors

    #region Methods

    protected override ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default) => new(token);

    #endregion Methods
}