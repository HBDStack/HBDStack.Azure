using System.Security;

namespace HBDStack.AzProxy.Core;

public static class CoreExtensions
{
    public static SecureString ToSecureString(this string plainString)
    {
        if (string.IsNullOrEmpty(plainString))
            return null;

        var secureString = new SecureString();

        foreach (char c in plainString.ToCharArray())
            secureString.AppendChar(c);

        return secureString;
    }
}