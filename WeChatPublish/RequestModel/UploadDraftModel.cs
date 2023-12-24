using System.Text.Json.Serialization;

namespace WeChatPublish.RequestModel;

public class UploadDraftModel
{
    [JsonPropertyName("articles")]
    public List<ArticleModel> Articles { get; set; } = new ();
}