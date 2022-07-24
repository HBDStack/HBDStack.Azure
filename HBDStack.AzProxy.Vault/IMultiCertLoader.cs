using System.Security.Cryptography.X509Certificates;

namespace HBDStack.AzProxy.Vault;

public interface IMultiCertLoader
{
    #region Methods

    ValueTask<X509Certificate2> GetCertAsync(string certName);

    #endregion Methods
}