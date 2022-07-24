using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.AzProxy.Apim.Tests;

[TestClass]
public class SetupTests
{
    #region Methods

    [TestMethod]
    public void Setup()
    {
        var p = new ServiceCollection()
            .AddApimProxies(Initialize.ResourceInfo, Initialize.Credential);

        p.Count.Should().Be(5);
    }

    #endregion Methods
}