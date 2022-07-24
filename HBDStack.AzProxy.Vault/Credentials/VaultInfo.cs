namespace HBDStack.AzProxy.Vault.Credentials;

public class VaultInfo
{
    #region Properties

    public SecretFilter Filter { get; set; } = new SecretFilter();

    public string VaultName { get; set; }

    public string VaultUri => $"https://{VaultName}.vault.azure.net";

    #endregion Properties
}