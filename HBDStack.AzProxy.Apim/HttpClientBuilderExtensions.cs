using HBDStack.AzProxy.Apim.Configs;
using HBDStack.AzProxy.Core;
using Microsoft.Extensions.Options;

namespace HBDStack.AzProxy.Apim;

public static class HttpClientBuilderExtensions
{
    #region Methods

    public static IHttpClientBuilder AddApimKeys(this IHttpClientBuilder builder, IOptions<ApimKeysConfig> config)
        => builder.AddApimKeys(config.Value);

    public static IHttpClientBuilder AddApimKeys(this IHttpClientBuilder builder, ApimKeysConfig config)
    {
        var p = builder.GetOrAddHeaderValuesProvider();

        p.Add(config.HeaderKey, config.SecretKey);
        return builder;
    }

    #endregion Methods
}