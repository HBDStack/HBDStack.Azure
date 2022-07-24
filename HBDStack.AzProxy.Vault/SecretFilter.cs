namespace HBDStack.AzProxy.Vault;

public sealed class SecretFilter
{
    #region Properties

    /// <summary>
    /// Mark this to true to load all secret from key vault.
    /// </summary>
    public bool All { get; set; }

    /// <summary>
    /// Load only secret which contains this value
    /// </summary>
    public string Contains { get; set; }

    /// <summary>
    /// Load only secret which ends with this value
    /// </summary>
    public string EndsWith { get; set; }

    /// <summary>
    /// Load only secret in this list
    /// </summary>
    public IList<string> Names { get; set; }

    /// <summary>
    /// Load only secret which starts with this value
    /// </summary>
    public string StartsWith { get; set; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Check whether name is matched with the filtering or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Matchs(string name)
    {
        if (Names != null && Names.Count > 0)
        {
            return Names.Any(n => n.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }

        //Ignore fillter if All is marked.
        if (All) return true;

        if (!string.IsNullOrEmpty(StartsWith) && !name.StartsWith(StartsWith, System.StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrEmpty(EndsWith) && !name.EndsWith(EndsWith, System.StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrEmpty(Contains) && !name.ToLowerInvariant().Contains(Contains.ToLowerInvariant()))
            return false;

        //Matched one of the rule above.
        return true;
    }

    #endregion Methods
}