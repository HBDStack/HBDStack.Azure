using System.Security.Cryptography.X509Certificates;

namespace HBDStack.AzProxy.Core.Providers;

public interface ICertProvider : IDisposable
{
    #region Methods

    ValueTask<X509Certificate2> GetCertAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}