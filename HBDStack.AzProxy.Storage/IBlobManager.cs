using Azure;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace HBDStack.AzProxy.Storage;

public interface IBlobManager
{
    #region Methods

    AsyncPageable<BlobHierarchyItem> GetBlobsByHierarchyAsync(BlobTraits traits = BlobTraits.None, BlobStates states = BlobStates.None, string delimiter = null, string prefix = null, CancellationToken cancellationToken = default);

    ValueTask<Uri> GetUrlAsync(string blobUrl, BlobSasPermissions permissions = BlobSasPermissions.Read, DateTimeOffset? expiresOn = null, CancellationToken cancellationToken = default);

    Task UploadAsync(string blobName, Stream data, CancellationToken cancellationToken = default);

    #endregion Methods
}