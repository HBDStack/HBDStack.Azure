namespace HBDStack.AzProxy.Storage;

public interface IAccountManager
{
    #region Methods

    Task<IBlobManager> GetOrCreateContainerAsync(string containerName);

    #endregion Methods
}