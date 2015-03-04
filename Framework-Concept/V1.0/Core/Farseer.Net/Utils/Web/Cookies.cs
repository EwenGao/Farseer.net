using System;
using System.Text.RegularExpressions;
using System.Web;
using FS.Configs;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     Cookies工具
    /// </summary>
    public abstract class Cookies
    {
        /// <summary>
        ///     写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="isAddPrefix">是否需要添加前缀</param>
        public static void Set(string strName, object strValue, bool isAddPrefix = true)
        {
            Set(strName, strValue, SystemConfigs.ConfigInfo.Cookies_TimeOut, isAddPrefix);
        }

        /// <summary>
        ///     写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="value">值</param>
        /// <param name="expires">过期时间(分钟)</param>
        /// <param name="isAddPrefix">是否需要添加前缀</param>
        public static void Set(string strName, object value, int expires, bool isAddPrefix = true)
        {
            if (isAddPrefix) { strName = SystemConfigs.ConfigInfo.Cookies_Prefix + strName; }
            if (value == null) { value = string.Empty; }
            var cookie = HttpContext.Current.Request.Cookies[strName] ?? new HttpCookie(strName);

            value = value.IsType<string>() ? HttpUtility.UrlEncode(value.ToString()) : value.ConvertType(string.Empty);

            cookie.Value = value.ToString();
            cookie.Expires = DateTime.Now.AddMinutes(expires);

            if (!GeneralConfigs.ConfigInfo.CookiesDomain.IsNullOrEmpty())
            {
                cookie.Domain = GeneralConfigs.ConfigInfo.CookiesDomain;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        ///     读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        /// <param name="isAddPrefix">是否需要添加前缀</param>
        public static string Get(string strName, bool isAddPrefix = true)
        {
            return Get(strName, string.Empty, isAddPrefix);
        }

        /// <summary>
        ///     读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isAddPrefix">是否需要添加前缀</param>
        public static T Get<T>(string strName, T defValue, bool isAddPrefix = true)
        {
            if (isAddPrefix) { strName = SystemConfigs.ConfigInfo.Cookies_Prefix + strName; }
            if (HttpContext.Current.Request.Cookies[strName] != null)
            {
                var httpCookie = HttpContext.Current.Request.Cookies[strName];
                if (httpCookie != null)
                {
                    var value = httpCookie.Value;

                    return HttpUtility.UrlDecode(value).ConvertType(defValue);
                }
            }
            return defValue;
        }

        /// <summary>
        ///     移除cookie值
        /// </summary>
        /// <param name="strName">名称c</param>
        /// <param name="isAddPrefix">是否需要添加前缀</param>
        public static void RemoveCookie(string strName, bool isAddPrefix = true)
        {
            Set(strName, "", -60 * 24, isAddPrefix);
        }

        /// <summary>
        ///     是否为有效域
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static bool IsValidDomain(string host)
        {
            var r = new Regex(@"^\d+$");
            if (host.IndexOf(".") == -1)
            {
                return false;
            }
            return r.IsMatch(host.Replace(".", string.Empty)) ? false : true;
        }
    }
}