using System.Security.Cryptography.X509Certificates;

namespace HBDStack.AzProxy.Core.Providers;

public abstract class CertProvider : ICertProvider
{
    #region Fields

    private X509Certificate2 cert = null;

    #endregion Fields

    #region Methods

    public virtual void Dispose() => cert?.Dispose();

    public async ValueTask<X509Certificate2> GetCertAsync(CancellationToken cancellationToken = default)
    {
        if (cert != null) return cert;
        cert = await InternalLoadCertAsync(cancellationToken);
        return cert;
    }

    protected abstract Task<X509Certificate2> InternalLoadCertAsync(CancellationToken cancellationToken);

    #endregion Methods
}