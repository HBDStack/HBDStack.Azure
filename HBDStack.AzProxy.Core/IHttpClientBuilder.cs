using HBDStack.AzProxy.Core.Providers;

namespace HBDStack.AzProxy.Core;

public interface IHttpClientBuilder
{
    #region Methods

    IHttpClientBuilder AddHeaderValuesProvider(IHeaderValuesProvider provider);

    IHttpClientBuilder ClientCertificate(ICertProvider certProvider);

    TProvider Get<TProvider>() where TProvider : IHeaderValuesProvider;

    TProvider Remove<TProvider>() where TProvider : IHeaderValuesProvider;

    IHttpClientBuilder SetCacheExpiration(TimeSpan expiration);

    #endregion Methods
}