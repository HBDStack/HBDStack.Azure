using Refit;
using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Core.AzAD;
using HBDStack.AzProxy.Apim.Credential;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim;

public static class ApimFactory
{
    #region Methods

    public static TProxy CreateIApimProxy<TProxy>(ApimResourceInfo apim, AppCredentials credential)
        where TProxy : IApimBase
    {
        var baseUrl =
            $"https://management.azure.com/subscriptions/{apim.SubscriptionId}/resourceGroups/{apim.ResourceGroupName}/providers/Microsoft.ApiManagement/service/{apim.ServiceName}";

        if (credential.Scope?.Any() != true)
            credential.Scope = new[] { "https://management.azure.com" };

        return AzProxyFactory.For<TProxy>(new Uri(baseUrl), builder => builder.UseAzAdToken(credential));
    }

    public static TError GetError<TError>(ApiException exception) where TError : IApimError
        => exception.Content != null
            ? System.Text.Json.JsonSerializer.Deserialize<TError>(exception.Content)
            : default;

    #endregion Methods
}