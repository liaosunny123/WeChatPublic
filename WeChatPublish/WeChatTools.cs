using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using RestSharp;
using WeChatPublish.RequestModel;
using WeChatPublish.UtilTools;

namespace WeChatPublish;

public static class WeChatTools
{
    public static bool TryGetGlobalAccessToken(GetAccessTokenModel model, out string accessToken)
    {
        accessToken = String.Empty;
        var client = new RestClient();
        var req = new RestRequest("https://api.weixin.qq.com/cgi-bin/stable_token", Method.Post);
        req.AddJsonBody(model);
        var res = client.Execute(req);
        var node = JsonNode.Parse(res.Content!);
        if (node != null && node["access_token"] != null)
        {
            accessToken = node["access_token"]!.ToString();
            return true;
        }
        return false;
    }
    
    public static bool TrySearchGood(SearchGoodsModel model, out GoodModel goodModel)
    {
        goodModel = new ();
        
        var client = new RestClient();
        var req = new RestRequest("https://daihuo.qq.com/trpc.cps.weixin_select.WeiXinSelect/Select", Method.Post);
        req.AddJsonBody(model);
        var res = client.Execute(req);
        if (!string.IsNullOrEmpty(res.Content))
        {
            goodModel = GoodModel.FromJson(res.Content!);
            return true;
        }
        return false;
    }
    
    public static bool TryUploadMedia(string accessToken, string pictureWebUrl, string type, out MediaModel media)
    {
        media = new MediaModel();
        
        var client = new RestClient();
        var downloadPicture = new RestRequest(pictureWebUrl);
        var random = new Random().Next(0, 114514191);
        if (!new DirectoryInfo(Directory.GetCurrentDirectory() + "\\cache").Exists)
        {
            new DirectoryInfo(Directory.GetCurrentDirectory() + "\\cache").Create();
        }
        File.WriteAllBytes(Directory.GetCurrentDirectory()+"\\cache\\"+ pictureWebUrl.GetSha1() + random +".jpg"
            ,client.Execute(downloadPicture).RawBytes!);
        string path = Directory.GetCurrentDirectory()+ "\\cache\\"+ pictureWebUrl.GetSha1() + random +".jpg";
        
        media = UploadFile("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token=ACCESS_TOKEN&type=TYPE",
            path, accessToken, "image");
        if (media == null)
        {
            return false;
        }

        return true;
    }
    
    private static MediaModel UploadFile(string URL, string filePath, string accessToken, string type)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found!");
        }

        string url = URL.Replace("ACCESS_TOKEN", accessToken).Replace("TYPE", type);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.KeepAlive = true;
        request.Credentials = CredentialCache.DefaultCredentials;

        string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
        request.ContentType = "multipart/form-data; boundary=" + boundary;

        StringBuilder sb = new StringBuilder();
        sb.Append("--").Append(boundary).Append("\r\n");
        sb.AppendFormat("Content-Disposition: form-data; name=\"media\"; filename=\"{0}\"; filelength={1}\r\n", Path.GetFileName(filePath), new FileInfo(filePath).Length);
        sb.Append("Content-Type: application/octet-stream\r\n\r\n");

        byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());
        byte[] fileData = File.ReadAllBytes(filePath);
        byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

        request.ContentLength = postHeaderBytes.Length + fileData.Length + boundaryBytes.Length;

        using (Stream postStream = request.GetRequestStream())
        {
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(fileData, 0, fileData.Length);
            postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
        }

        string result = null;
        using (WebResponse response = request.GetResponse())
        {
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            result = reader.ReadToEnd();
        }

        var json = JsonNode.Parse(result);
        
        string mediaId = type != "image" ? json[type + "_media_id"].ToString() : json["media_id"].ToString();
        
        return new MediaModel()
        {
            MediaId = mediaId,
            MediaUrl = json["url"].ToString()
        };
    }
    
    public static bool TryUploadDraft(ArticleModel model, string accessToken, out string mediaId)
    {
        mediaId = String.Empty;
        var client = new RestClient();
        var req = new RestRequest($"https://api.weixin.qq.com/cgi-bin/draft/add?access_token={accessToken}", Method.Post);
        var options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        
        req.AddJsonBody(JsonSerializer.Serialize(new UploadDraftModel()
        {
            Articles = new List<ArticleModel>()
            {
                model
            }
        },options));
        var res = client.Execute(req);
        var node = JsonNode.Parse(res.Content!);
        if (node != null && node["media_id"] != null)
        {
            mediaId = node["media_id"]!.ToString();
            return true;
        }
        return false;
    }
    
    public static bool TryPush(PublishModel model, string accessToken)
    {
        var client = new RestClient();
        var req = new RestRequest($"https://api.weixin.qq.com/cgi-bin/freepublish/submit?access_token={accessToken}", Method.Post);
        req.AddJsonBody(model);
        var res = client.Execute(req);
        var node = JsonNode.Parse(res.Content!);
        if (node != null && node["errcode"] != null && node["errcode"].ToString() == "0")
        {
            return true;
        }
        return false;
    }
}