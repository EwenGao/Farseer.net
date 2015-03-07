using System.Reflection;
using System.Text;
using System.Web;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     Mvc请求上下文
    /// </summary>
    public abstract class MvcReq
    {
        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public static string QS(string parmsName)
        {
            return QS(parmsName, string.Empty);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public static T QS<T>(string parmsName, T defValue = default(T))
        {
            if (HttpContext.Current == null)
            {
                return defValue;
            }
            var value = HttpContext.Current.Request.RequestContext.RouteData.Values[parmsName] ?? string.Empty;
            return Url.UrlDecode(value.ToString()).ConvertType(defValue);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public static string QS(string parmsName, Encoding encoding)
        {
            return QS(parmsName, string.Empty, encoding);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public static T QS<T>(string parmsName, T defValue = default(T), Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (HttpContext.Current == null)
            {
                return defValue;
            }
            var requestType = HttpContext.Current.Request.RequestContext.GetType();
            var property = requestType.GetProperty("QueryStringBytes",
                                                            BindingFlags.Instance | BindingFlags.IgnoreCase |
                                                            BindingFlags.NonPublic);
            var queryBytes = (byte[]) property.GetValue(HttpContext.Current.Request, null);

            var parms = HttpUtility.UrlDecode(queryBytes, encoding);
            if (parms.IsNullOrEmpty())
            {
                return defValue;
            }

            parmsName += "=";
            foreach (var str in parms.Split('&'))
            {
                if (str.IsStartsWith(parmsName))
                {
                    return str.Substring(parmsName.Length).ConvertType(defValue);
                }
            }
            return defValue;
        }

        /// <summary>
        ///     获取当前应用程序访问的页面文件名称(不带参数)
        ///     Default.aspx
        /// </summary>
        public static string GetPageName()
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return string.Empty;
            }

            var url = context.Request.FilePath;
            url = Url.GetPageName(url);
            return url;
        }

        /// <summary>
        ///     获取当前应用程序访问的参数
        /// </summary>
        public static string GetParams()
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return string.Empty;
            }

            var url = context.Request.RawUrl;
            return Url.GetParams(url);
        }
    }
}