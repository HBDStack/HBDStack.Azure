using System.Runtime.CompilerServices;

namespace HBDStack.AzProxy.Core.Providers;

public class HeaderValuesProvider : IHeaderValuesProvider
{
    #region Fields

    private readonly IDictionary<string, Func<ValueTask<string>>> factories;
    private readonly IDictionary<string, string> values;

    #endregion Fields

    #region Constructors

    public HeaderValuesProvider(params KeyValuePair<string, string>[] values)
    {
        factories = new Dictionary<string, Func<ValueTask<string>>>();
        this.values = new Dictionary<string, string>();

        foreach (var item in values)
            this.values.Add(item);
    }

    #endregion Constructors

    #region Methods

    public void Add(string key, string value) => values.Add(key, value);

    public void Add(string key, Func<ValueTask<string>> factory) => factories.Add(key, factory);

    public async IAsyncEnumerable<KeyValuePair<string, string>> GetHeaderAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var v in values)
        {
            yield return v;
        }

        foreach (var f in factories)
        {
            var v = await f.Value();
            yield return new KeyValuePair<string, string>(f.Key, v);
        }
    }

    #endregion Methods
}