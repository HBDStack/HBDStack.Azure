using System.Runtime.CompilerServices;

namespace HBDStack.AzProxy.Core.Providers;

public abstract class TokenProvider : IHeaderValuesProvider
{
    #region Methods

    public async IAsyncEnumerable<KeyValuePair<string, string>> GetHeaderAsync([EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var token = await GetTokenAsync(cancellationToken);
        yield return new KeyValuePair<string, string>(HeaderKeys.Authorization, token);
    }

    protected abstract ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}