using Refit;
using System.Text.Json;

namespace HBDStack.AzProxy.Core;

public static class AzProxyFactory
{
    private static readonly RefitSettings Default = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    };


    public static TProxy For<TProxy>(Uri uri, Action<IHttpClientBuilder> builder, Func<HttpClient> factory = null, RefitSettings refitSettings = null)
    {
        var handler = new ProvidersHttpClientHandler();
        builder(handler);

        return ForWithHandler<TProxy>(uri, handler, factory, refitSettings);
    }

    public static TProxy ForWithHandler<TProxy>(Uri uri, HttpClientHandler handler, Func<HttpClient> factory = null, RefitSettings refitSettings = null)
    {
        var client = factory?.Invoke() ?? new HttpClient(handler) { BaseAddress = uri };
        var settings = refitSettings ?? Default;

        return RestService.For<TProxy>(client, settings);
    }
}