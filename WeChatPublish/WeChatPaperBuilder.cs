using System.Text;

namespace WeChatPublish;

public class WeChatPaperBuilder
{
    private StringBuilder _stringBuilder = new();

    public void AddWord(string word)
    {
        _stringBuilder.Append($"<p>{word}</p>");
    }
    
    public void AddGood(string goodId, bool isListTemplate = true)
    {
        var template = isListTemplate ? "list" : "default";
        _stringBuilder.Append($"<p><mpcps contenteditable=\"false\" class=\"js_editor_new_cps\" data-templateid=\"{template}\" data-goodssouce=\"1\" data-pid=\"{goodId}\"></mpcps></p>");
    }

    public void AddImg(string url)
    {
        _stringBuilder.Append($"<p><img data-type=\"jpeg\" data-src=\"{url}\"></p>");
    }

    public string Build()
    {
        return _stringBuilder.ToString();
    }
}