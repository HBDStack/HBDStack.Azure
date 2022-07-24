using Refit;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim;

public interface IApimGroupApi : IApimBase
{
    #region Methods

    [Put("/groups/{groupId}/users/{userId}/?api-version=2019-01-01")]
    Task AddUser(string groupId, string userId);

    [Put("/groups/{groupId}?api-version=2019-01-01")]
    Task<ApimGroup> CreateOrUpdate(string groupId, [Body]ApimGroup group);

    [Get("/groups/{groupId}/?api-version=2019-01-01")]
    Task<ApimGroup> Get(string groupId);

    [Get("/groups/?api-version=2019-01-01")]
    Task<ApimResult<ApimGroup>> GetAll();

    [Delete("/groups/{groupId}/users/{userId}/?api-version=2019-01-01")]
    Task RemoveUser(string groupId, string userId);

    #endregion Methods
}