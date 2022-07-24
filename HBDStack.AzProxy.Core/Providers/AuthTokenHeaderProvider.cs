using System.Runtime.CompilerServices;

namespace HBDStack.AzProxy.Core.Providers;

public abstract class AuthTokenHeaderProvider : IAuthTokenHeaderProvider
{
    #region Methods

    public async IAsyncEnumerable<KeyValuePair<string, string>> GetHeaderAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var authKey = HeaderKeys.Authorization;
        var token = await this.GetTokenAsync();

        yield return new KeyValuePair<string, string>(authKey, token);
    }

    protected abstract ValueTask<string> GetTokenAsync();

    #endregion Methods
}