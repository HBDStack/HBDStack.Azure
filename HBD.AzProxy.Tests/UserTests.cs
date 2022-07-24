using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refit;
using System.Threading.Tasks;
using HBDStack.AzProxy.Apim.Dto;
using HBDStack.AzProxy.Core.AzAD;

namespace HBDStack.AzProxy.Apim.Tests;

[TestClass] [Ignore]
public class UserTests
{
    #region Methods

    [TestMethod]
    public async Task CreateUserAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimUserApi>(Initialize.ResourceInfo, Initialize.Credential);

        try
        {
            var user = await api.CreateOrUpdate($"{System.Guid.NewGuid()}-{System.Guid.NewGuid()}", new ApimUser
            {
                Properties = new ApimUserProperties
                {
                    FirstName = "Duy",
                    LastName = "Hoang",
                    Email = "baoduy2412@yahoo.com"
                }
            });

            user.Should().NotBeNull();
        }
        catch (ApiException ex)
        {
            Assert.Fail(ex.Message + "\n" + ex.Content);
        }
    }

    [TestMethod]
    public async Task GetTokenAsync()
    {
        var context = new AuthContext(Initialize.Credential);
        var token = await context.AcquireTokenAsync();

        token.Should().NotBeNull();
        token.AccessToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task GetUserAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimUserApi>(Initialize.ResourceInfo, Initialize.Credential);

        var user = await api.Get("1");
        user.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetUsersAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimUserApi>(Initialize.ResourceInfo, Initialize.Credential);

        var users = await api.GetAll();
        users.Value.Should().NotBeNullOrEmpty();
    }

    #endregion Methods
}