using System.Text.Json.Serialization;

namespace WeChatPublish.RequestModel;

public class GetAccessTokenModel
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = "client_credential";
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = String.Empty;
    [JsonPropertyName("secret")]
    public string Secret { get; set; } = String.Empty;
    [JsonPropertyName("force_refresh")]
    public bool ForceRefresh { get; set; } = true;
}