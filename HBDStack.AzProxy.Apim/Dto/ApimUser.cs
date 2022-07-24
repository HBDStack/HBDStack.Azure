namespace HBDStack.AzProxy.Apim.Dto;

public sealed class ApimUser : ApimItem<ApimUserProperties>
{
}

public class ApimUserIdentity
{
    #region Properties

    public string Id { get; set; }

    public string Provider { get; set; }

    #endregion Properties
}

public class ApimUserProperties
{
    #region Properties

    public string Email { get; set; }

    public string FirstName { get; set; }

    public IList<ApimUserIdentity> Identities { get; set; }

    public string LastName { get; set; }

    public string Note { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string State { get; set; }

    #endregion Properties
}