using Refit;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim;

public interface IApimApi : IApimBase
{
    #region Methods

    [Get("/apis/{apiId}/?api-version=2019-01-01")]
    Task<ApimApi> Get(string apiId);

    [Get("/apis/?api-version=2019-01-01")]
    Task<ApimResult<ApimApi>> GetAll();

    #endregion Methods
}