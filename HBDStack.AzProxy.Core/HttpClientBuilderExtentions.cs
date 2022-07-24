using System.Security.Cryptography.X509Certificates;
using HBDStack.AzProxy.Core.AzAD;
using HBDStack.AzProxy.Core.Providers;

namespace HBDStack.AzProxy.Core;

public static class HttpClientBuilderExtentions
{
    #region Methods

    /// <summary>
    /// Add <see cref="IAuthTokenHeaderProvider"/> allow to inject <see cref="HeaderKeys.Authorization"/> to Headers
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddAuthToken(this IHttpClientBuilder builder, IAuthTokenHeaderProvider provider)
        => builder.AddHeaderValuesProvider(provider);

    public static IHttpClientBuilder AddHeaderValue(this IHttpClientBuilder builder, string name, string value)
    {
        var p = builder.GetOrAddHeaderValuesProvider();

        p.Add(name, value);
        return builder;
    }

    public static IHttpClientBuilder AddHeaderValue(this IHttpClientBuilder builder, string name, Func<ValueTask<string>> factory)
    {
        var p = builder.GetOrAddHeaderValuesProvider();

        p.Add(name, factory);
        return builder;
    }

    public static IHttpClientBuilder DisableCache(this IHttpClientBuilder builder)
        => builder.SetCacheExpiration(TimeSpan.FromSeconds(1));

    public static HeaderValuesProvider GetOrAddHeaderValuesProvider(this IHttpClientBuilder builder)
    {
        var p = builder.Get<HeaderValuesProvider>();
        if (p == null)
        {
            p = new HeaderValuesProvider();
            builder.AddHeaderValuesProvider(p);
        }
        return p;
    }

    public static IHttpClientBuilder UseAzAdToken(this IHttpClientBuilder builder, AppCredentials credential)
        => builder.AddHeaderValuesProvider(new AzAdTokenProvider(credential));

    public static IHttpClientBuilder UseClientCert(this IHttpClientBuilder builder, string certFile, string certPass = null)
        => builder.ClientCertificate(new CertFileProvider(certFile, certPass));

    public static IHttpClientBuilder UseClientCert(this IHttpClientBuilder builder, string subjecctNameOrThumbprint, StoreName storeName, StoreLocation location = StoreLocation.LocalMachine)
        => builder.ClientCertificate(new CertStorageProvider(subjecctNameOrThumbprint, storeName, location));

    public static IHttpClientBuilder UseToken(this IHttpClientBuilder builder, string token)
        => builder.AddHeaderValuesProvider(new StaticAuthTokenProvider(token));

    #endregion Methods
}