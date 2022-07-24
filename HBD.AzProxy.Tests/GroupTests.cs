using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refit;
using System.Threading.Tasks;
using HBDStack.AzProxy.Apim.Dto;

namespace HBDStack.AzProxy.Apim.Tests;

[TestClass]
[Ignore]
public class GroupTests
{
    #region Methods

    [TestMethod]
    public async Task AddUserToGroup()
    {
        var api = ApimFactory.CreateIApimProxy<IApimGroupApi>(Initialize.ResourceInfo, Initialize.Credential);
        await api.AddUser("luminor", "754faab2-ceea-4cb6-95a9-28190e021d35-d29f1cdd-c829-43ca-a6a1-8fd9b66533fa");
    }

    [TestMethod]
    public async Task CreateGroupAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimGroupApi>(Initialize.ResourceInfo, Initialize.Credential);

        try
        {
            var user = await api.CreateOrUpdate($"luminor", new ApimGroup
            {
                Name = "luminor",
                Properties = new ApimGroupProperties
                {
                    Description = "The Group of Luminor Application.",
                    DisplayName = "Luminor App",
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
    public async Task GetGroupAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimGroupApi>(Initialize.ResourceInfo, Initialize.Credential);

        var group = await api.Get("Administrators");
        group.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetGroupsAsync()
    {
        var api = ApimFactory.CreateIApimProxy<IApimGroupApi>(Initialize.ResourceInfo, Initialize.Credential);

        var groups = await api.GetAll();
        groups.Value.Should().NotBeNullOrEmpty();
    }

    #endregion Methods
}