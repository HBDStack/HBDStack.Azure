using FluentAssertions;
using HBDStack.AzProxy.Vault.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.AzProxy.Vault.Tests;

[TestClass]
public class TestVaultCertLoader
{
    #region Methods

    [TestMethod]
    public async System.Threading.Tasks.Task TestLoadCertAsync()
    {
        var loader = new VaultCertProvider("ClientAuthCert", Initialize.VaultInfo, Initialize.VaultCredential);
        var cert = await loader.GetCertAsync();

        cert.Should().NotBeNull();
        cert.Thumbprint.Should().NotBeNullOrEmpty();
    }

    #endregion Methods
}