using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;

namespace WeChatPublish.UtilTools;

public static class UtilsToolGroup
{
    public static string TextGainCenter(string left, string right, string text) {
        //判断是否为null或者是empty
        if (string.IsNullOrEmpty(left))
            return "";
        if (string.IsNullOrEmpty(right))
            return "";
        if (string.IsNullOrEmpty(text))
            return "";

        int Lindex = text.IndexOf(left); //搜索left的位置
            
        if (Lindex == -1){ //判断是否找到left
            return "";
        }
        //abcd a d
        Lindex = Lindex + left.Length; //取出left右边文本起始位置
            
        int Rindex = text.IndexOf(right, Lindex);//从left的右边开始寻找right
           
        if (Rindex == -1){//判断是否找到right
            return "";   
        }
            
        return text.Substring(Lindex, Rindex - Lindex);//返回查找到的文本
    }

    public static List<string> TextGainCenterBatch(string left, string right, string text)
    {
        var list = new List<string>();
        while (true)
        {
            var match = TextGainCenter(left, right, text);
            if (string.IsNullOrEmpty(match))
            {
                break;
            }
            
            list.Add(match);
            text = text.Replace(left + match + right, "");
        }
        return list;
    }
    
    /// <summary>
    /// 获取视频时长
    /// </summary>
    /// <param name="sourceFile">视频地址</param>
    /// <param name="ffmpegfile">ffmpeg存放文件夹地址</param>
    /// <returns></returns>
    public static string GetVideoDuration(string sourceFile,string ffmpegfile)
    {
        using (System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process())
        {
            String duration; 
            String result;
            StreamReader errorreader; 
            ffmpeg.StartInfo.UseShellExecute = false;
            //ffmpeg.StartInfo.ErrorDialog = false;
            ffmpeg.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.FileName = ffmpegfile;
            ffmpeg.StartInfo.Arguments = "-i " + sourceFile;
            ffmpeg.StartInfo.CreateNoWindow = true;// 不显示程序窗口
            ffmpeg.Start();
            errorreader = ffmpeg.StandardError;
            ffmpeg.WaitForExit();
            result = errorreader.ReadToEnd();
            duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);
            return duration;
        }
    }

    /// <summary>
    /// 导出封面图
    /// </summary>
    /// <param name="ffmpegFileName">FFmpeg.exe路径</param>
    /// <param name="videoFileName">视频文件路径</param>
    /// <returns>封面图</returns>
    public static string GetVideoFace(string ffmpegFileName, string videoFileName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoFileName);
        string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "video_tmp");
        if (!Directory.Exists(baseDirectory)) Directory.CreateDirectory(baseDirectory);
        string thumbFileName = Path.Combine(baseDirectory, fileNameWithoutExtension + ".jpg");
        ProcessStartInfo processStartInfo = new ProcessStartInfo(ffmpegFileName);
        processStartInfo.UseShellExecute = false;
        processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processStartInfo.CreateNoWindow = true;
        processStartInfo.ErrorDialog = false;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.Arguments = string.Format("-i \"{0}\" -y -f image2 -frames 30 \"{1}\"", new object[]
        {
            videoFileName,
            thumbFileName
        }); // 第30帧
        Process.Start(processStartInfo)!.WaitForExit(500);
        if (File.Exists(thumbFileName)) return thumbFileName;
        return "";
    }

    public static void Log(string msg)
    {
        File.AppendAllText(Directory.GetCurrentDirectory()+"\\log.log",msg+"\n");
    }

    public static void Success(string name, string msg)
    {
        File.AppendAllText(Directory.GetCurrentDirectory()+$"\\{name}_success.log",msg+"\n");
    }
    
    public static void Error(string name, string msg)
    {
        File.AppendAllText(Directory.GetCurrentDirectory()+$"\\{name}_error.log",msg+"\n");
    }
    
    public static string GetPictureCs(string sourceFile)
    {
        System.Diagnostics.Process pro = new System.Diagnostics.Process();
        pro.StartInfo.FileName = "cmd.exe";
        pro.StartInfo.UseShellExecute = false;
        pro.StartInfo.RedirectStandardError = true;
        pro.StartInfo.RedirectStandardInput = true;
        pro.StartInfo.RedirectStandardOutput = true;
        pro.StartInfo.CreateNoWindow = true;
        //pro.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        pro.Start();
        pro.StandardInput.WriteLine("node index.js --path=\"" + sourceFile + "\"");
        pro.StandardInput.WriteLine("exit");
        pro.StandardInput.AutoFlush = true;
        //获取cmd窗口的输出信息
        string output = pro.StandardOutput.ReadToEnd();
        pro.WaitForExit();//等待程序执行完退出进程
        pro.Close();
        return output.Split("\r\n")[4].Split("\n")[0];
    }


    public static string MD5Value(String filepath)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] md5ch;
        using (FileStream fs = File.OpenRead(filepath))
        {
            md5ch = md5.ComputeHash(fs);
        }

        md5.Clear();
        string strMd5 = "";
        for (int i = 0; i < md5ch.Length; i++)
        {
            strMd5 += md5ch[i].ToString("x").PadLeft(2, '0');
        }

        return strMd5;
    }

    public static string GetUnixTime(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds().ToString();
    }
}