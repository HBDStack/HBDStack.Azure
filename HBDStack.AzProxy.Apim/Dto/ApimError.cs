namespace HBDStack.AzProxy.Apim.Dto;

public interface IApimError { }

public sealed class ApimError : IApimError
{
    #region Properties

    public ApimErrorDetails Error { get; set; }

    #endregion Properties
}

public sealed class ApimErrorDetails
{
    #region Properties

    public string Code { get; set; }

    public IList<ApimErrorDetails> Details { get; } = new List<ApimErrorDetails>();

    public string Message { get; set; }

    public string Target { get; set; }

    #endregion Properties
}