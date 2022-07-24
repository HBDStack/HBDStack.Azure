using HBDStack.AzProxy.Core;
using HBDStack.AzProxy.Vault.Credentials;

namespace HBDStack.AzProxy.Vault.Tests;

public class Initialize
{
    #region Properties

    public static AppClientCredentials VaultCredential => new(
        "ee79b6d9-ff81-48f2-85ad-db852b33b198",
        "5ecY6hZn*d3IG?kyQuTrKO~DVD9%eo?e+F*WptVS9~UHBLUS^4"
    );

    public static VaultInfo VaultInfo => new() { VaultName = "dev-root-vault", Filter = new SecretFilter { StartsWith = "dev" } };

    #endregion Properties
}