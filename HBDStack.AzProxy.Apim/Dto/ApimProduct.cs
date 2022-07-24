namespace HBDStack.AzProxy.Apim.Dto;

public sealed class ApimProduct : ApimItem<ApimProductProperties>
{
}

public class ApimProductProperties
{
    #region Properties

    public bool ApprovalRequired { get; set; }

    public string Description { get; set; }

    public string DisplayName { get; set; }

    public string State { get; set; }

    public bool SubscriptionRequired { get; set; }

    #endregion Properties
}