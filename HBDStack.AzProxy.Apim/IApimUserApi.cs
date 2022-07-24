using Refit;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim;

public interface IApimUserApi : IApimBase
{
    #region Methods

    [Put("/users/{userId}?api-version=2019-01-01")]
    Task<ApimUser> CreateOrUpdate(string userId, [Body]ApimUser user);

    [Get("/users/{userId}/?api-version=2019-01-01")]
    Task<ApimUser> Get(string userId);

    [Get("/users/?api-version=2019-01-01")]
    Task<ApimResult<ApimUser>> GetAll();

    #endregion Methods
}