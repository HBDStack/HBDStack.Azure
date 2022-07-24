using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;

namespace HBDStack.AzProxy.Core;

public static class JwtDecode
{
    #region Methods

    public static JwtToken Decode(string token)
    {
        var t = new JwtSecurityToken(token);
        var raw = t.ToString();
        var rs = System.Text.Json.JsonSerializer.Deserialize<JwtToken>("{" + raw.Split("}.{")[1])!;

        rs.IssuedAt = t.IssuedAt;
        rs.ValidFrom = t.ValidFrom;
        rs.ValidTo = t.ValidTo;

        return rs;
    }

    #endregion Methods
}

public class JwtToken
{
    #region Properties

    [JsonPropertyName("appid")]
    public string AppId { get; set; }

    [JsonPropertyName("app_displayname")]
    public string AppName { get; set; }

    [JsonPropertyName("aud")]
    public string Audience { get; set; }

    [JsonPropertyName("iss")]
    public string Authority { get; set; }

    public DateTime IssuedAt { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("nonce")]
    public string Nonce { get; set; }

    [JsonPropertyName("oid")]
    public string ObjectId { get; set; }

    [JsonPropertyName("tid")]
    public string TenantId { get; set; }

    [JsonPropertyName("preferred_username")]
    public string UserName { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    [JsonPropertyName("ver")]
    public string Version { get; set; }

    #endregion Properties
}