namespace HBDStack.AzProxy.Apim.Configs;

/// <summary>
/// This is use to load the configuration from appSettings.json
/// </summary>
public class ApimKeysConfig
{
    #region Fields

    /// <summary>
    /// The section name in appSettings.json
    /// </summary>
    public static string Name = "Apim";

    #endregion Fields

    #region Properties

    public string HeaderKey { get; set; }

    public string SecretKey { get; set; }

    #endregion Properties
}