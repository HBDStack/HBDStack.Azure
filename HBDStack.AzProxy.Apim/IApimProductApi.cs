using Refit;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim;

public interface IApimProductApi : IApimBase
{
    #region Methods

    [Get("/products/{pid}/?api-version=2019-01-01")]
    Task<ApimProduct> Get(string pid);

    [Get("/products/?api-version=2019-01-01")]
    Task<ApimResult<ApimProduct>> GetAll();

    #endregion Methods
}