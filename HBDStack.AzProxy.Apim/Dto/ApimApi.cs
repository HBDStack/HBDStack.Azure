namespace HBDStack.AzProxy.Apim.Dto;

public sealed class ApimApi : ApimItem<ApimApiProperties>
{
}

public class ApimApiProperties
{
    #region Properties

    public string ApiRevision { get; set; }

    public string AuthenticationSettings { get; set; }

    public string Description { get; set; }

    public string DisplayName { get; set; }

    public string Path { get; set; }

    public string ServiceUrl { get; set; }

    public string SubscriptionKeyParameterNames { get; set; }

    public bool SubscriptionRequired { get; set; }

    #endregion Properties
}