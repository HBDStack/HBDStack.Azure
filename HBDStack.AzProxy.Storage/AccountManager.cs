using Azure.Storage.Blobs;
using System.Globalization;

namespace HBDStack.AzProxy.Storage;

public class AccountManager : IAccountManager
{
    #region Fields

    private readonly string connectionString;

    #endregion Fields

    #region Constructors

    public AccountManager(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        this.connectionString = connectionString;
    }

    #endregion Constructors

    #region Methods

    public async Task<IBlobManager> GetOrCreateContainerAsync(string containerName)
    {
        var container = new BlobContainerClient(connectionString, containerName.ToLower(CultureInfo.CurrentCulture));
        await container.CreateIfNotExistsAsync().ConfigureAwait(false);
        return new BlobManager(container, connectionString);
    }

    #endregion Methods
}