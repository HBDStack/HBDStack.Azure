using Newtonsoft.Json;

namespace HBDStack.AzProxy.Core.Tests;

public partial class Model
{
    #region Properties

    [JsonProperty("amount")]
    public long Amount { get; set; }

    [JsonProperty("buyCountryCode")]
    public string BuyCountryCode { get; set; }

    [JsonProperty("buyCurrency")]
    public string BuyCurrency { get; set; }

    [JsonProperty("membershipType")]
    public string MembershipType { get; set; }

    [JsonProperty("profileTier")]
    public string ProfileTier { get; set; }

    [JsonProperty("sellCountryCode")]
    public string SellCountryCode { get; set; }

    [JsonProperty("sellCurrency")]
    public string SellCurrency { get; set; }

    #endregion Properties
}