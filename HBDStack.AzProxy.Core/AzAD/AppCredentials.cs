using HBDStack.AzProxy.Core.Providers;

namespace HBDStack.AzProxy.Core.AzAD;

/// <summary>
/// The credentials with clientId, clientSecret or Certfificate
/// </summary>
public class AppCredentials : AppClientCredentials
{
    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="authority">tenant Id or authority Url</param>
    /// <param name="scope"></param>
    public AppCredentials(string authority, AppClientCredentials credentials = null)
        : base(credentials) => SetValue(authority);

    /// <summary>
    ///
    /// </summary>
    /// <param name="authority">tenant Id or authority Url</param>
    /// <param name="clientId"></param>
    /// <param name="secret"></param>
    /// <param name="scope"></param>
    public AppCredentials(string authority, string clientId, string secret)
        : base(clientId, secret) => SetValue(authority);

    /// <summary>
    ///
    /// </summary>
    /// <param name="authority">tenant Id or authority Url</param>
    /// <param name="clientId"></param>
    /// <param name="certProvider"></param>
    /// <param name="scope"></param>
    public AppCredentials(string authority, string clientId, ICertProvider certProvider)
        : base(clientId, certProvider) => SetValue(authority);

    #endregion Constructors

    #region Properties

    public string Authority { get; private set; }

    /// <summary>
    /// The default scope of the token
    /// </summary>
    public string[] Scope { get; set; }
    #endregion Properties

    #region Methods

    private void SetValue(string authority) => Authority = authority.StartsWith("http") ? authority : $"https://login.microsoftonline.com/{authority}/";

    #endregion Methods
}