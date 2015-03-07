using System;
using System.Net;
using System.Text;

namespace FS.Utils
{
    /// <summary>
    ///     支持 Session 和 Cookie 的 WebClient。
    /// </summary>
    public class HttpClient : WebClient
    {
        // Cookie 容器
        private CookieContainer cookieContainer;

        /// <summary>
        ///     创建一个新的 WebClient 实例。
        /// </summary>
        public HttpClient()
        {
            cookieContainer = new CookieContainer();
        }

        /// <summary>
        ///     创建一个新的 WebClient 实例。
        /// </summary>
        /// <param name="cookies">Cookie 容器</param>
        public HttpClient(CookieContainer cookies)
        {
            cookieContainer = cookies;
        }

        /// <summary>
        ///     Cookie 容器
        /// </summary>
        public CookieContainer Cookies
        {
            get { return cookieContainer; }
            set { cookieContainer = value; }
        }

        /// <summary>
        ///     返回带有 Cookie 的 HttpWebRequest。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                var httpRequest = request as HttpWebRequest;
                httpRequest.CookieContainer = cookieContainer;
            }
            return request;
        }

        #region 封装了PostData, GetSrc 和 GetFile 方法

        /// <summary>
        ///     向指定的 URL POST 数据，并返回页面
        /// </summary>
        /// <param name="url">POST URL</param>
        /// <param name="postString">POST 的 数据</param>
        /// <param name="encoding">POST 数据的 CharSet</param>
        /// <param name="msg">页面的 CharSet</param>
        /// <returns>页面的源文件</returns>
        public string PostData(string url, string postString, Encoding encoding, out string msg)
        {
            try
            {
                // 将 Post 字符串转换成字节数组
                var postData = encoding.GetBytes(postString);
                Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                // 上传数据，返回页面的字节数组
                var responseData = UploadData(url, "POST", postData);
                // 将返回的将字节数组转换成字符串(HTML);
                var srcString = encoding.GetString(responseData);
                srcString = srcString.Replace("\t", "");
                srcString = srcString.Replace("\r", "");
                srcString = srcString.Replace("\n", "");
                msg = string.Empty;
                return srcString;
            }
            catch (WebException we)
            {
                msg = we.Message;
                return string.Empty;
            }
        }

        /// <summary>
        ///     获得指定 URL 的源文件
        /// </summary>
        /// <param name="uriString">页面 URL</param>
        /// <param name="dataEncoding">页面的 CharSet</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>页面的源文件</returns>
        public string GetSrc(string uriString, string dataEncoding, out string errorMessage)
        {
            try
            {
                // 返回页面的字节数组
                var responseData = DownloadData(uriString);
                // 将返回的将字节数组转换成字符串(HTML);
                var srcString = Encoding.GetEncoding(dataEncoding).GetString(responseData);
                srcString = srcString.Replace("\t", "");
                srcString = srcString.Replace("\r", "");
                srcString = srcString.Replace("\n", "");
                errorMessage = string.Empty;
                return srcString;
            }
            catch (WebException we)
            {
                errorMessage = we.Message;
                return string.Empty;
            }
        }

        /// <summary>
        ///     从指定的 URL 下载文件到本地
        /// </summary>
        /// <param name="urlString">文件 URL</param>
        /// <param name="fileName">本地文件的完成路径</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns></returns>
        public bool GetFile(string urlString, string fileName, out string errorMessage)
        {
            try
            {
                DownloadFile(urlString, fileName);
                errorMessage = string.Empty;
                return true;
            }
            catch (WebException we)
            {
                errorMessage = we.Message;
                return false;
            }
        }

        #endregion
    }
}