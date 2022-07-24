using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refit;
using System;
using System.Threading.Tasks;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim.Tests;

[TestClass] [Ignore]
public class SubcriptionTests
{
    #region Methods

    [TestMethod]
    public async Task CreateSubAsync()
    {
        var useApi = ApimFactory.CreateIApimProxy<IApimUserApi>(Initialize.ResourceInfo, Initialize.Credential);
        var productApi = ApimFactory.CreateIApimProxy<IApimProductApi>(Initialize.ResourceInfo, Initialize.Credential);
        var api = ApimFactory.CreateIApimProxy<IApimSubscriptionApi>(Initialize.ResourceInfo, Initialize.Credential);

        try
        {
            var user = await useApi.Get("754faab2-ceea-4cb6-95a9-28190e021d35-d29f1cdd-c829-43ca-a6a1-8fd9b66533fa");
            var product = await productApi.Get("sg-dev-luminor");

            var sub = await api.CreateOrUpdate(Guid.NewGuid().ToString(), new ApimSubscription
            {
                Name = "sg-dev-luminor-duy",
                Properties = new ApimSubscriptionProperties
                {
                    OwnerId = user.Id,
                    Scope = product.Id,
                    DisplayName = "sg-dev-luminor-duy",
                }
            });

            sub.Should().NotBeNull();
        }
        catch (ApiException ex)
        {
            Assert.Fail(ex.Message + "\n" + ex.Content);
        }
    }

    [TestMethod]
    public async Task GetSubAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimSubscriptionApi>(Initialize.ResourceInfo, Initialize.Credential);

        var sub = await api.Get("master");
        sub.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetSubsAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimSubscriptionApi>(Initialize.ResourceInfo, Initialize.Credential);

        var subs = await api.GetAll();
        subs.Value.Should().NotBeNullOrEmpty();
    }

    #endregion Methods
}