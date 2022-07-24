namespace HBDStack.AzProxy.Apim.Dto;

public sealed class ApimGroup : ApimItem<ApimGroupProperties>
{
}

public class ApimGroupProperties
{
    #region Properties

    public string Description { get; set; }

    public string DisplayName { get; set; }

    #endregion Properties
}