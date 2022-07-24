using System.Security.Cryptography.X509Certificates;

namespace HBDStack.AzProxy.Core.Providers;

/// <summary>
/// Load Certificate from file.
/// </summary>
public class CertFileProvider : CertProvider

{
    #region Fields

    private readonly string certPath;
    private readonly string password;

    #endregion Fields

    #region Constructors

    public CertFileProvider(string certPath, string password = null)
    {
        this.certPath = certPath;
        this.password = password;
    }

    #endregion Constructors

    #region Methods

    protected override Task<X509Certificate2> InternalLoadCertAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = TryGetPath();
        if (path == null) return Task.FromResult<X509Certificate2>(null);

        var certBytes = File.ReadAllBytes(path);

        cancellationToken.ThrowIfCancellationRequested();

        var cert = new X509Certificate2(certBytes, string.IsNullOrWhiteSpace(password) ? null : password,
            X509KeyStorageFlags.MachineKeySet);

        return Task.FromResult(cert);
    }

    private string TryGetPath()
    {
        if (string.IsNullOrEmpty(certPath)) return null;

        if (File.Exists(certPath)) return certPath;

        var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(CertFileProvider).Assembly.Location)!, certPath));

        if (File.Exists(path)) return path;

        return null;
    }

    #endregion Methods
}