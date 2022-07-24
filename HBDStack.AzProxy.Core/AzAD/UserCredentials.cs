using System.Security;

namespace HBDStack.AzProxy.Core.AzAD;

public class UserCredentials
{
    public UserCredentials(string userName, SecureString password)
    {
        UserName = userName;
        Password = password;
    }

    public UserCredentials(string userName, string password)
    {
        UserName = userName;
        Password = password.ToSecureString();
    }

    public string UserName { get; }
    public SecureString Password { get; }

}