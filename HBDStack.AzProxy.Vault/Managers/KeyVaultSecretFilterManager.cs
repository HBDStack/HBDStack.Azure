using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace HBDStack.AzProxy.Vault.Managers;

public class KeyVaultSecretFilterManager : DefaultKeyVaultSecretManager
{
    #region Fields

    private readonly SecretFilter filter;

    #endregion Fields

    #region Constructors

    public KeyVaultSecretFilterManager(SecretFilter filter) => this.filter = filter ?? throw new ArgumentNullException(nameof(filter));

    #endregion Constructors

    #region Methods

    public override bool Load(SecretItem secret)
        => filter.Matchs(secret.Identifier.Name);

    #endregion Methods
}