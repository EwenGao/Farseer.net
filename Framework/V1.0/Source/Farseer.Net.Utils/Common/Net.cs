using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using FS.Extend;
using FS.Utils.Web;

namespace FS.Utils.Common
{
    /// <summary>
    ///     下载文件
    /// </summary>
    public abstract class Net
    {
        /// <summary>
        ///     下载小文件
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="wc"></param>
        public static int Save(string url, string savePath, WebClient wc)
        {
            url = Url.ConvertPath(url);
            if (url.IsNullOrEmpty()) { return 0; }
            if (url[0] == '/') { url = Req.GetDomain() + url; }

            var fileSize = 0;
            var isNew = wc == null;
            if (wc == null)
            {
                wc = new WebClient();
                wc.Proxy = null;
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Referer", Url.UrlEncode(url));
                wc.Headers.Add("Cookie", "bid=\"YObnALe98pw\";");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
            }

            try
            {
                wc.DownloadFile(url, savePath);
                var f = new FileInfo(savePath);
                fileSize = (int)f.Length;
                f = null;
            }
            catch
            {
                fileSize = 0;
            }
            finally
            {
                if (!isNew) wc.Cookies();
                else { wc.Dispose(); wc = null; }
            }
            return fileSize;
        }

        /// <summary>
        ///     下载小文件
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="wc"></param>
        public static int Save(string url, string savePath)
        {
            url = Url.ConvertPath(url);
            if (url.IsNullOrEmpty()) { return 0; }
            if (url[0] == '/') { url = Req.GetDomain() + url; }

            var fileSize = 0;

            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Referer", Url.UrlEncode(url));
                wc.Headers.Add("Cookie", "bid=\"YObnALe98pw\";");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");

                try
                {
                    wc.DownloadFile(url, savePath);
                    var f = new FileInfo(savePath);
                    fileSize = (int)f.Length;
                    f = null;
                }
                catch { fileSize = 0; }
            }
            return fileSize;
        }

        /// <summary>
        /// 下载文件到客户端
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public static void DownFile(string filePath, string fileName)
        {
            var Response = HttpContext.Current.Response;

            //指定块大小   
            long chunkSize = 102400;
            //建立一个100K的缓冲区   
            var buffer = new byte[chunkSize];
            //已读的字节数   
            long dataToRead = 0;
            FileStream stream = null;
            try
            {
                //打开文件   
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataToRead = stream.Length;
                //添加Http头   
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachement;filename=" + fileName);
                Response.AddHeader("Content-Length", dataToRead.ToString());
                while (dataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        var length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));
                        Response.OutputStream.Write(buffer, 0, length);
                        Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead -= length;
                    }
                    else
                    {
                        //防止client失去连接  
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex) { Response.Write("Error:" + ex.Message); }
            finally
            { if (stream != null) { stream.Close(); } Response.Close(); }
        }

        /// <summary>
        /// 下载文件到客户端
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public static void DownStream(string s, string fileName, string contentType = "application/octet-stream")
        {
            var response = HttpContext.Current.Response;

            response.ContentEncoding = Encoding.UTF8;
            response.AppendHeader("content-disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + "\"");
            response.ContentType = contentType;

            response.Write(s);
            response.End();
        }

        /// <summary>
        ///     获取远程信息
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="encoding">请求编码</param>
        /// <param name="wc">客户端</param>
        public static string Get(string url, Encoding encoding = null, WebClient wc = null)
        {
            url = Url.ConvertPath(url);
            if (url.IsNullOrEmpty()) { return string.Empty; }
            if (url[0] == '/') { url = Req.GetDomain() + url; }
            if (encoding == null) encoding = Encoding.UTF8;
            var isNew = wc == null;
            if (wc == null)
            {
                wc = new WebClient();
                wc.Proxy = null;
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Referer", Url.UrlEncode(url));
                wc.Headers.Add("Cookie", "bid=\"YObnALe98pw\";");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
            }
            string strResult = null;
            try
            {
                var data = wc.DownloadData(url);
                strResult = encoding.GetString(data);
            }
            catch { return string.Empty; }
            finally
            {
                if (!isNew) wc.Cookies();
                if (isNew) wc.Dispose();
            }
            return strResult;
        }

        /// <summary>
        ///     传入URL返回网页的html代码
        /// </summary>
        /// <param name="url">要读取的网页URL</param>
        /// <param name="readCode">读取源文件所使用的编码</param>
        /// <param name="cookie">传过去的cookie</param>
        public static string Get(string url, Encoding encoding, ref CookieContainer cookie)
        {
            url = Url.ConvertPath(url);
            if (url[0] == '/') { url = Req.GetDomain() + url; }
            if (url.IsNullOrEmpty()) { return string.Empty; }
            if (encoding == null) { encoding = Encoding.UTF8; }

            var content = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = null;
                request.KeepAlive = false;
                request.CookieContainer = cookie;
                //request.Headers.Add("Accept", "*/*");
                //request.Headers.Add("Referer", url);
                request.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");

                var response = (HttpWebResponse)request.GetResponse();
                response.Cookies = cookie.GetCookies(request.RequestUri);

                var respStream = response.GetResponseStream();
                if (respStream != null)
                {
                    var reader = new StreamReader(respStream, encoding);
                    content = reader.ReadToEnd();
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                content = "error:" + ex.Message;
            }
            return content;
        }

        /// <summary>
        ///     Post信息
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="queryString">QueryString</param>
        /// <param name="encoding">请求编码</param>
        /// <param name="wc">Web客户端</param>
        public static string UploadData(string url, string queryString, Encoding encoding = null, WebClient wc = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            var isNew = false;
            if (wc == null)
            {
                wc = new WebClient();
                wc.Proxy = null;
                isNew = true;
            }
            string strResult = null;
            try
            {
                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                var dataResult = wc.UploadData(url, null, encoding.GetBytes(queryString));
                strResult = encoding.GetString(dataResult);
                wc.Headers.Remove("Content-Type");
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var reader = new StreamReader(ex.Response.GetResponseStream(), encoding);
                    strResult = string.Format("Error:{0}\n<hr />\n{1}", ex.Message, reader.ReadToEnd());
                }
            }
            finally
            {
                if (!isNew) wc.Cookies();
                if (isNew) wc.Dispose();
            }

            return strResult;
        }

        /// <summary>
        ///     判断网络文件是否存在
        /// </summary>
        /// <param name="url">要读取的网页URL</param>
        /// <param name="readCode">读取源文件所使用的编码</param>
        public static bool IsHaving(string url, Encoding encoding = null)
        {
            url = Url.ConvertPath(url);
            if (url.IsNullOrEmpty()) { return false; }
            if (url[0] == '/') { url = Req.GetDomain() + url; }
            if (encoding == null) { encoding = Encoding.UTF8; }

            bool isHaving;
            try
            {
                using (var web = new WebClient())
                {
                    web.Proxy = null;
                    web.Headers.Add("Accept", "*/*");
                    web.Headers.Add("Referer", url);
                    web.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                    web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
                    isHaving = web.DownloadData(url).Length > 0;
                }
            }
            catch { isHaving = false; }
            return isHaving;
        }

        public static string Post(string url, string postData)
        {
            var encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(postData);

            var myRequest = (HttpWebRequest)WebRequest.Create(url);

            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            using (var newStream = myRequest.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
                newStream.Close();
            }

            using (var myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                var reader = new StreamReader(myResponse.GetResponseStream(), Encoding.Default);
                return reader.ReadToEnd();
            }
        }

    }
}