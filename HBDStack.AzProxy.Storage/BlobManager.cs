using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace HBDStack.AzProxy.Storage;

public class BlobManager : IBlobManager
{
    #region Fields

    private readonly string connectionString;
    private readonly BlobContainerClient containerClient;

    #endregion Fields

    #region Constructors

    public BlobManager(BlobContainerClient containerClient, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        this.containerClient = containerClient ?? throw new ArgumentNullException(nameof(containerClient));
        this.connectionString = connectionString;
    }

    #endregion Constructors

    #region Methods

    public AsyncPageable<BlobHierarchyItem> GetBlobsByHierarchyAsync(BlobTraits traits = BlobTraits.None, BlobStates states = BlobStates.None, string delimiter = null, string prefix = null, CancellationToken cancellationToken = default)
        => containerClient.GetBlobsByHierarchyAsync(traits, states, delimiter, prefix, cancellationToken);

    /// <summary>
    /// Get Url with Sas token. Default expiresOn is 1 year, Default permission is readonly
    /// </summary>
    /// <param name="blobUrl"></param>
    /// <param name="permissions"></param>
    /// <param name="expiresOn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<Uri> GetUrlAsync(string blobUrl, BlobSasPermissions permissions = BlobSasPermissions.Read, DateTimeOffset? expiresOn = null, CancellationToken cancellationToken = default)
    {
        var blob = containerClient.GetBlobClient(blobUrl);

        if (permissions.HasFlag(BlobSasPermissions.Read))
        {
            if (!(await blob.ExistsAsync(cancellationToken).ConfigureAwait(false)).Value)
                throw new FileNotFoundException(blobUrl);
        }

        // Create a SAS token that's valid for one hour.
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobUrl,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = expiresOn ?? DateTimeOffset.UtcNow.AddYears(1)
        };

        // Specify read permissions for the SAS.
        sasBuilder.SetPermissions(permissions);

        // Use the key to get the SAS token.
        var key = AzStorageExtensions.GetAccountKey(connectionString);
        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(containerClient.AccountName, key)).ToString();

        // Construct the full URI, including the SAS token.
        return new UriBuilder(blob.Uri) { Query = sasToken }.Uri;
    }

    public async Task UploadAsync(string blobName, Stream data, CancellationToken cancellationToken = default)
        => await containerClient.UploadBlobAsync(blobName, data, cancellationToken).ConfigureAwait(false);

    #endregion Methods
}