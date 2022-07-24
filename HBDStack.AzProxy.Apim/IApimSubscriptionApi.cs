using Refit;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim;

public interface IApimSubscriptionApi : IApimBase
{
    #region Methods

    [Put("/subscriptions/{sid}?api-version=2019-01-01")]
    Task<ApimSubscription> CreateOrUpdate(string sid, [Body]ApimSubscription subscription);

    [Get("/subscriptions/{sid}/?api-version=2019-01-01")]
    Task<ApimSubscription> Get(string sid);

    [Get("/subscriptions/?api-version=2019-01-01")]
    Task<ApimResult<ApimSubscription>> GetAll();

    [Post("/subscriptions/{sid}/regeneratePrimaryKey?api-version=2019-01-01")]
    Task RegeneratePrimaryKey(string sid);

    [Post("/subscriptions/{sid}/regenerateSecondaryKey?api-version=2019-01-01")]
    Task RegenerateSecondaryKey(string sid);

    #endregion Methods
}