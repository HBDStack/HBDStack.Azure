using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using HBDStack.AzProxy.Apim.Credential;

namespace HBDStack.AzProxy.Apim.Handlers;

public static class SasTokenCreator
{
    #region Methods

    public static string NewSas(ApimCredential credential, TimeSpan expiry = default)
    {
        var ex = expiry == default ? DateTime.UtcNow.AddMinutes(30) : DateTime.UtcNow.Add(expiry);

        using (var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(credential.Key)))
        {
            var dataToSign = credential.Identifier + "\n" + ex.ToString("O", CultureInfo.InvariantCulture);
            var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            var signature = Convert.ToBase64String(hash);
            var encodedToken = string.Format("SharedAccessSignature uid={0}&ex={1:o}&sn={2}", credential.Identifier, ex, signature);

            return encodedToken;
        }
    }

    #endregion Methods
}