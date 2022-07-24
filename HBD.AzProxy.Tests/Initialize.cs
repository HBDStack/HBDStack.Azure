using HBDStack.AzProxy.Apim.Credential;
using HBDStack.AzProxy.Core.AzAD;

namespace HBDStack.AzProxy.Apim.Tests;

public class Initialize
{
    #region Properties

    public static AppCredentials Credential =>
        new("67a0d28d-ea8b-4f59-b7a3-e49cd0638b56",
            "aaeda077-3be3-46ec-b676-474da8d93262",
            "2m6%HF#+4X+*JO_tqx%O~+<h-0iV?:bkROMRzY68d7<4chn1&k");

    public static ApimResourceInfo ResourceInfo => new()
    {
        SubscriptionId = "63a31b41-eb5d-4160-9fc9-d30fc00286c9",
        ResourceGroupName = "sg-sandbox-api-Transwap",
        ServiceName = "sg-sandbox-api-Transwap",
    };

    #endregion Properties
}