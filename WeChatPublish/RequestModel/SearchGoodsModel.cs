namespace WeChatPublish.RequestModel;
using System;
using System.Text.Json.Serialization;


public class SearchGoodsModel
{
    [JsonPropertyName("uin")]
    public string Uin { get; set; }

    [JsonPropertyName("page")]
    public Page Page { get; set; }

    [JsonPropertyName("scene")]
    public long Scene { get; set; }

    [JsonPropertyName("product_query")]
    public ProductQuery ProductQuery { get; set; }

    [JsonPropertyName("link_query")]
    public LinkQuery LinkQuery { get; set; }
}

public class LinkQuery
{
    [JsonPropertyName("link")]
    public string[] Link { get; set; }
}

public class Page
{
    [JsonPropertyName("no")]
    public long No { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }
}

public class ProductQuery
{
    [JsonPropertyName("article_template")]
    public bool ArticleTemplate { get; set; }

    [JsonPropertyName("search_type")]
    public long SearchType { get; set; }
}
