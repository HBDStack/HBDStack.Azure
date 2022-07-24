namespace HBDStack.AzProxy.Apim.Dto;

public interface IApimItem
{
    #region Properties

    string Id { get; set; }

    string Name { get; set; }

    string Type { get; set; }

    #endregion Properties
}

public class ApimItem<TProperties> : IApimItem where TProperties : class
{
    #region Properties

    public string Id { get; set; }

    public string Name { get; set; }

    public TProperties Properties { get; set; }

    public string Type { get; set; }

    #endregion Properties
}

public class ApimResult<TValue> where TValue : IApimItem
{
    #region Properties

    public IList<TValue> Value { get; set; }

    #endregion Properties
}