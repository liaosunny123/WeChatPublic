using System.Text.Json.Serialization;

namespace WeChatPublish.RequestModel;

public class PublishModel
{
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = String.Empty;
}