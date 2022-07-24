using HBDStack.AzProxy.Apim;
using HBDStack.AzProxy.Apim.Configs;
using HBDStack.AzProxy.Apim.Credential;
using HBDStack.AzProxy.Core.AzAD;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApimSetup
{
    #region Methods

    public static IServiceCollection AddApimConfigures(this IServiceCollection services, IConfiguration configuration)
        => services.Configure<ApimKeysConfig>(configuration.GetSection(ApimKeysConfig.Name));

    public static IServiceCollection AddApimProxies(this IServiceCollection services, ApimResourceInfo apim, AppCredentials credential)
    {
        return services
                .AddSingleton(ApimFactory.CreateIApimProxy<IApimApi>(apim, credential))
                .AddSingleton(ApimFactory.CreateIApimProxy<IApimGroupApi>(apim, credential))
                .AddSingleton(ApimFactory.CreateIApimProxy<IApimProductApi>(apim, credential))
                .AddSingleton(ApimFactory.CreateIApimProxy<IApimSubscriptionApi>(apim, credential))
                .AddSingleton(ApimFactory.CreateIApimProxy<IApimUserApi>(apim, credential))
            ;
    }

    #endregion Methods
}