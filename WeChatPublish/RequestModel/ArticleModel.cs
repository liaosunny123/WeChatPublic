using System.Text.Json.Serialization;

namespace WeChatPublish.RequestModel;

public class ArticleModel
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = String.Empty;
    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; } = String.Empty;
    [JsonPropertyName("author")]
    public string Author { get; set; } = String.Empty;
    [JsonPropertyName("digest")]
    public string Digest { get; set; } = String.Empty;
    [JsonPropertyName("show_cover_pic")]
    public string ShowCoverPic { get; set; } = String.Empty;
    [JsonPropertyName("content")]
    public string Content { get; set; } = String.Empty;
    [JsonPropertyName("content_source_url")]
    public string ContentSourceUrl { get; set; } = String.Empty;
    [JsonPropertyName("need_open_comment")]
    public int NeedOpenComment { get; set; } = 0;
    [JsonPropertyName("only_fans_can_comment")]
    public int OnlyFanCanComment { get; set; } = 0;
}