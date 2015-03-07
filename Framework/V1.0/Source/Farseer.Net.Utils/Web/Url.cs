using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     解释Url
    /// </summary>
    public abstract class Url
    {
        /// <summary>
        ///     对Url字符，进去编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        ///     对Url字符，进去解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        ///     检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl,
                                 @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        ///     返回完全域名，带端口
        /// </summary>
        /// <param name="url">来源URL</param>
        /// <returns></returns>
        public static string GetDomain(string url)
        {
            var newUrl = url.Replace("\\", "/").ClearString("http://", RegexOptions.IgnoreCase);
            return "http://" + newUrl.Substring(0, newUrl.IndexOf('/'));
        }

        /// <summary>
        ///     获取当前应用程序访问的域名(带端口)
        ///     www.xxx.com:81
        /// </summary>
        /// <param name="node">要获取第几个节点，0：不限制</param>
        public static string GetDomain(string url, int node)
        {
            var newUrl = url.Replace("\\", "/").ClearString("http://", RegexOptions.IgnoreCase);
            newUrl = newUrl.Substring(0, newUrl.IndexOf('/'));

            if (node < 1) { return url; }

            var lstDomain = newUrl.ToList("", ".");
            while (lstDomain.Count > node) { lstDomain.RemoveAt(0); }
            return lstDomain.ToString(".");
        }

        /// <summary>
        ///     获得当前页面的名称
        /// </summary>
        public static string GetPageName(string url)
        {
            url = url.Split('?')[0];
            url = url.Substring(url.LastIndexOf('/') + 1);
            return url.Contains<char>('.') ? url : string.Empty;
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static string GetParams(string url)
        {
            if (url.IsNullOrEmpty()) { return string.Empty; }
            var paramIndex = url.IndexOf('?');
            if (paramIndex == -1 || url == "?") { return url; }
            return url.Substring(paramIndex + 1);

            ////url = Url.UrlDecode(url);
            ////清除重复的参数
            //List<string> lstParms = new List<string>();
            //foreach (var param in url.Substring(paramIndex + 1).Split('&'))
            //{
            //    string[] parmsSplit = param.Split('=');
            //    if (parmsSplit.Length == 1) { continue; }

            //    if (!lstParms.Exists(o => o.IsStartsWith(parmsSplit[0])))
            //    {
            //        lstParms.Add(string.Format("{0}={1}", parmsSplit[0], parmsSplit[1]));
            //    }
            //}
            //return lstParms.ToString("&");
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static string GetParm(string url, string paramName)
        {
            return GetParm(url, paramName, string.Empty);
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static T GetParm<T>(string url, string paramName, T defVal)
        {
            var strParams = GetParams(url);
            foreach (var param in strParams.Split('&'))
            {
                var parmsSplit = param.Split('=');
                if (parmsSplit.Length == 1) { continue; }

                if (parmsSplit[0].IsEquals(paramName)) { return parmsSplit[1].ConvertType(defVal); }
            }

            return defVal;
        }

        /// <summary>
        ///     参数编码
        /// </summary>
        public static string ParmsEncode(string parms)
        {
            var lstParms = new List<string>();
            foreach (var strs in parms.Split('&'))
            {
                var index = strs.IndexOf('=');
                if (index > -1)
                {
                    lstParms.Add(strs.SubString(0, index + 1) + UrlEncode(strs.SubString(index + 1, -1)));
                }
            }
            return lstParms.ToString("&");
        }

        /// <summary>
        ///     将相对路径，转换成决对路径
        /// </summary>
        /// <param name="requestUrl">http请求的地址</param>
        /// <param name="url">html获得的地址</param>
        /// <returns></returns>
        public static string ConvertUrlToDomain(string requestUrl, string url)
        {
            url = Url.ConvertPath(url);
            // 绝对路径，直接返回
            if (url.IsStartsWith("http://"))
            {
                return url;
            }
            // 相对绝对路径，添加域名后返回
            if (url.IsStartsWith("/"))
            {
                return GetDomain(requestUrl) + url;
            }

            #region 相对路径 如../../

            // 深度
            var parentCount = 0;
            // 计算深度
            while (url.IsStartsWith("../"))
            {
                parentCount++;
                url = url.Substring(3);
            }
            // 初始化请求的地址
            if (requestUrl.EndsWith("/") || requestUrl.LastIndexOf('.') > requestUrl.LastIndexOf('/'))
            {
                requestUrl = requestUrl.DelLastOf("/");
            }
            //根据深度，得到目录
            while (parentCount > 0)
            {
                parentCount--;
                requestUrl = requestUrl.DelLastOf("/");
            }
            return requestUrl + "/" + url;

            #endregion
        }

        /// <summary>
        ///     将\\转换成：/
        /// </summary>
        /// <returns></returns>
        public static string ConvertPath(string path)
        {
            return path.IsNullOrEmpty() ? string.Empty : path.Replace("\\", "/");
        }

        /// <summary>
        /// 通过正则，获取IP
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetIP(string str)
        {
            return new Regex("\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}").Match(str).Value;
        }
    }
}