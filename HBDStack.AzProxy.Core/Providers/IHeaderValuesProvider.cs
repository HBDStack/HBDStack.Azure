namespace HBDStack.AzProxy.Core.Providers;

public interface IHeaderValuesProvider
{
    #region Methods

    public IAsyncEnumerable<KeyValuePair<string, string>> GetHeaderAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}