using FluentAssertions;
using HBDStack.AzProxy.Core.AzAD;
using HBDStack.AzProxy.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Threading.Tasks;

namespace HBDStack.AzProxy.Core.Tests;

[TestClass]
public class UnitTests
{
    private static readonly AppCredentials Credentials = new("https://login.microsoftonline.com/a43302d1-4dce-481d-b034-a38776605989", "71cbd042-1b94-4e48-a3a6-1fb589379f36", ".T48igbVsCW1n-h2I.13jqVL.qJ_J033z~");
    private static readonly UserCredentials UserCredentials = new("drunktest@drunkcoding.net", "!9XJUj4U3qzqZ2iC$jd61dHsV65WSiR6Tb3HM25&nI4H*Gqcnh");

    #region Methods

    [TestMethod]
    public async Task LoginWithUserNameAndPassword()
    {
        var auth = new AuthContext(Credentials);
        var token = await auth.AcquireTokenByUsernamePassword(UserCredentials, new[] { "https://graph.microsoft.com/.default" });

        token.IdToken.Should().NotBeNullOrEmpty();
        token.AccessToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task AcquireTokenAsync()
    {
        var auth = new AuthContext(Credentials);
        var token = await auth.AcquireTokenAsync();

        token.AccessToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task GetGraphTokenFromJwtAsync()
    {
        var auth = new AuthContext(Credentials);
        var idtoken = await auth.AcquireTokenByUsernamePassword(UserCredentials);

        var token = await auth.AcquireTokenOnBehalfOfAsync(idtoken.IdToken ?? idtoken.AccessToken, new[] { "https://graph.microsoft.com/.default" });
        token.AccessToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task GetTokenWithCertificateAsync()
    {
        var auth = new AuthContext(new AppCredentials(
            Credentials.Authority,
            Credentials.ClientId,
            new CertFileProvider("ClientAuthCert")));

        var token = await auth.AcquireTokenAsync(new[] { "https://graph.microsoft.com/.default" });
        token.AccessToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task LoadCertFromFileAsync()
    {
        var loader = new CertFileProvider("ClientAuthCert");
        var cert = await loader.GetCertAsync();
        cert.Should().NotBeNull();
    }

    [TestMethod]
    [Ignore]
    public async Task LoadCertFromStoreAsync()
    {
        var loader = new CertStorageProvider("a43489159a520f0d93d032ccaf37e7fe20a8b419");
        var cert = await loader.GetCertAsync();
        cert.Should().NotBeNull();
    }

    [TestMethod]
    public async Task TestAuthTokenHeaderProviderAsync()
    {
        var mock = new Mock<AuthTokenHeaderProvider>();
        mock.Protected().Setup<ValueTask<string>>("GetTokenAsync").Returns(new ValueTask<string>("MyToken")).Verifiable();

        await foreach (var c in mock.Object.GetHeaderAsync())
        {
            c.Key.Should().Be(HeaderKeys.Authorization);
            c.Value.Should().Be("MyToken");
        }
        mock.VerifyAll();
    }

    [TestMethod]
    public async Task TestAuthTokenHeaderShouldBeInHttpClientAsync()
    {
        var mock = new Mock<AuthTokenHeaderProvider>();
        mock.Protected().Setup<ValueTask<string>>("GetTokenAsync").Returns(new ValueTask<string>("MyToken")).Verifiable();

        var sv =AzProxyFactory.For<IDeepSeaApi>(new Uri("https://deapsea.com"), b => b.AddHeaderValuesProvider(mock.Object));

        try
        {
            await sv.Post(new Model());
        }
        catch
        {
            // ignored
        }

        mock.Protected().Verify("GetTokenAsync", Times.Once());
    }

    #endregion Methods
}