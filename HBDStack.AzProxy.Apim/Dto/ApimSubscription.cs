namespace HBDStack.AzProxy.Apim.Dto;

public sealed class ApimSubscription : ApimItem<ApimSubscriptionProperties>

{
}

public class ApimSubscriptionProperties
{
    #region Properties

    public bool AllowTracing { get; set; } = true;

    public string DisplayName { get; set; }

    public string OwnerId { get; set; }

    public string PrimaryKey { get; set; }

    public string Scope { get; set; }

    public string SecondaryKey { get; set; }

    #endregion Properties
}