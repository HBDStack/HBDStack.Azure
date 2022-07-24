using FluentAssertions;
using HBDStack.AzProxy.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBD.AzProxy.Storage.Tests;

[TestClass]
public class StorageExtensionsTests
{
    #region Methods

    [TestMethod]
    public void Test_ExtractConnectionStringValue()
    {
        var key = AzStorageExtensions.GetAccountKey(Consts.ConnectionString);
        key.Should().NotBeNullOrEmpty();
        key.Should().Be("09FfJNl7oCWiQIx8yvrjRQeyoin4a4VtLiz7QE/+uqJbqzVYix5GizArt15nt83AN7U7TfcET5QUayBJlYINfw==");
    }

    #endregion Methods
}