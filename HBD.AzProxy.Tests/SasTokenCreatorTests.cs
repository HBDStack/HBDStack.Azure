using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HBDStack.AzProxy.Apim.Credential;
using HBDStack.AzProxy.Apim.Handlers;

namespace HBDStack.AzProxy.Apim.Tests;

[TestClass]
public class SasTokenCreatorTests
{
    #region Methods

    [TestMethod]
    public void CreateSas()
    {
        var sas = SasTokenCreator.NewSas(new ApimCredential { Identifier = "Duy", Key = Guid.NewGuid().ToString() });

        sas.Should().StartWith("SharedAccessSignature")
            .And.Contain("uid=Duy")
            .And.Contain("&sn=");
    }

    #endregion Methods
}