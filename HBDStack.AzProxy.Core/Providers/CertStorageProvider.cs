using System.Security.Cryptography.X509Certificates;

namespace HBDStack.AzProxy.Core.Providers;

/// <summary>
/// Load Certificate from Computer Storage
/// </summary>
public class CertStorageProvider : CertProvider
{
    #region Fields

    private readonly StoreLocation location;
    private readonly StoreName storeName;
    private readonly string subjecctNameOrThumbprint;

    #endregion Fields

    #region Constructors

    public CertStorageProvider(string subjecctNameOrThumbprint, StoreName storeName = StoreName.Root, StoreLocation location = StoreLocation.LocalMachine)
    {
        this.subjecctNameOrThumbprint = subjecctNameOrThumbprint;
        this.storeName = storeName;
        this.location = location;
    }

    #endregion Constructors

    #region Methods

    protected override Task<X509Certificate2> InternalLoadCertAsync(CancellationToken cancellationToken = default)
    {
        using var store = new X509Store(storeName, location);
        store.Open(OpenFlags.MaxAllowed);

        cancellationToken.ThrowIfCancellationRequested();
        var results = store.Certificates.Find(X509FindType.FindByThumbprint, subjecctNameOrThumbprint, true);

        cancellationToken.ThrowIfCancellationRequested();
        if (results.Count == 0)
            results = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subjecctNameOrThumbprint, true);

        cancellationToken.ThrowIfCancellationRequested();
        if (results.Count == 0)
            results = store.Certificates.Find(X509FindType.FindBySubjectName, subjecctNameOrThumbprint, true);

        cancellationToken.ThrowIfCancellationRequested();
        return results.Count == 0 ? Task.FromResult<X509Certificate2>(null) : Task.FromResult(results[0]);
    }

    #endregion Methods
}