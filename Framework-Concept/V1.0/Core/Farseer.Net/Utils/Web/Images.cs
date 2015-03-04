using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;

namespace FS.Utils.Web
{
    /// <summary>
    ///     图片工具
    /// </summary>
    public abstract class Images
    {
        /// <summary>
        ///     保存图片
        /// </summary>
        public static void SaveImageByRequest(string imageUrl, string savePath, ImageFormat imgFormat)
        {
            var wreq = WebRequest.Create(imageUrl);
            var wresp = (HttpWebResponse) wreq.GetResponse();
            var s = wresp.GetResponseStream();
            var img = Image.FromStream(s);
            img.Save(savePath, imgFormat); //保存 
        }

        /// <summary>
        ///     保存图片
        /// </summary>
        public static void SaveImageByClient(string imageUrl, string savePath, ImageFormat imgFormat)
        {
            var my = new WebClient();
            var mybyte = my.DownloadData(imageUrl);
            var ms = new MemoryStream(mybyte);
            var img = Image.FromStream(ms);
            img.Save(savePath, imgFormat); //保存
        }

        /// <summary>
        ///     将二进制代码输出图片
        /// </summary>
        public static void WriteImage(byte[] buffer)
        {
            //下面直接输出 
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/gif";
            HttpContext.Current.Response.BinaryWrite(buffer);
        }
    }
}