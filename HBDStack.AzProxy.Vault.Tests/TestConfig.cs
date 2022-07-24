using FluentAssertions;
using HBDStack.AzProxy.Vault.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.AzProxy.Vault.Tests;

[TestClass]
public class TestConfig
{
    #region Methods

    [TestMethod]
    public void TestConfigBuilder()
    {
        var config = new ConfigurationBuilder()
            .AddVaults(Initialize.VaultInfo, Initialize.VaultCredential)
            .Build();

        config["dev-log-wp"].Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void TestConfigBuilderWithFile()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("config.json", optional: false)
            .AddVaults()
            .Build();

        config["Luminor-ClientId"].Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async System.Threading.Tasks.Task TestServiceCollectionWithFileAsync()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("config.json", optional: false)
            .AddVaults()
            .Build();

        var service = new ServiceCollection()
            .AddVaultServices(config)
            .BuildServiceProvider();

        var c = service.GetRequiredConfigure<VaultConfig>();
        c.Value.ClientId.Should().NotBeNull();

        var p = service.GetRequiredService<IMultiCertLoader>();
        (await p.GetCertAsync("ClientAuthCert")).Should().NotBeNull();

        var v = service.GetVaultCertProvider("ClientAuthCert");
        (await v.GetCertAsync()).Should().NotBeNull();
    }

    #endregion Methods
}