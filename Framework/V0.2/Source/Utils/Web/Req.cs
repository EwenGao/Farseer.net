using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using FS.Configs;
using FS.Core.Model;
using FS.Extend;
using FS.Mapping.Verify;
using FS.Utils.Common;

namespace FS.Utils.Web
{
    /// <summary>
    ///     Request操作类
    /// </summary>
    public abstract class Req
    {
        /// <summary>
        ///     请求类型
        /// </summary>
        public enum SubmitType
        {
            /// <summary>
            ///     Get
            /// </summary>
            Get = 0,

            /// <summary>
            ///     Post
            /// </summary>
            Post = 1,

            /// <summary>
            ///     Get 和 Post (Post优先)
            /// </summary>
            All = 255
        }

        /// <summary>
        ///     获取当前应用程序访问的完整路径信息
        ///     http://localhost:1480/WebSite2/Default.aspx?UserID=1
        /// </summary>
        /// <returns></returns>
        public static string GetUrl()
        {
            return HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        ///     获取当前应用程序访问的域名(带端口)
        ///     www.xxx.com:81
        /// </summary>
        /// <param name="node">要获取第几个节点，0：不限制</param>
        public static string GetDomain(int node = 3)
        {
            var domain = HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.Url.Authority;
            if (node < 1) { return domain; }

            var lstDomain = domain.ToList("", ".");
            while (lstDomain.Count > node) { lstDomain.RemoveAt(0); }
            return lstDomain.ToString(".");
        }

        /// <summary>
        ///     获取当前应用程序访问的路径(不带域名、不带页面文件名、URL重写地址的真实路径)
        ///     /Manage/
        /// </summary>
        public static string GetTruePath()
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return string.Empty;
            }

            var url = context.Request.FilePath;
            if (url.Contains<char>('.'))
            {
                url = url.Substring(0, url.LastIndexOf("/") + 1);
            }
            if (url.Length != url.LastIndexOf('/') + 1)
            {
                url += "/";
            }
            return url.ToLower();
        }

        /// <summary>
        /// 获取当前应用程序访问的路径(不带域名、不带页面文件名)
        /// /Manage/
        /// </summary>
        public static string GetPath()
        {
            var context = HttpContext.Current;
            if (context == null) { return string.Empty; }

            var url = context.Request.RawUrl.SubString("?");
            url = url.Substring(0, url.LastIndexOf("/") + 1);

            if (url.Length != url.LastIndexOf('/') + 1) { url += "/"; }
            return url.ToLower();
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
            if (context == null) { return string.Empty; }
            var dic = new Dictionary<string, string>();
            var parm = Url.GetParams(context.Request.RawUrl).Split('&');
            foreach (var item in parm)
            {
                if (!item.Contains("=")) { continue; }
                var items = item.Split('=');
                dic[items[0]] = context.Request.QueryString[items[0]];
            }
            return ParseParams(dic);

            //for (int i = 0; i < context.Request.QueryString.Count; i++)
            //{
            //    str.AppendFormat("&{0}={1}", context.Request.QueryString.GetKey(i), context.Request.QueryString[i]);
            //}
            //return str.Length > 0 ? str.ToString().Substring(1) : string.Empty;
            //return Url.GetParms(context.Request.RawUrl);
        }

        /// <summary>
        /// 解析成参数字符串
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ParseParams(Dictionary<string, string> dic)
        {
            var lst = new List<string>();
            foreach (var item in dic) { lst.Add(item.Key + "=" + item.Value); }
            return lst.ToString("&");
        }

        /// <summary>
        ///     上一个页面地址
        /// </summary>
        public static string GetPrevious()
        {
            return HttpContext.Current == null ? string.Empty : Convert.ToString(((object)HttpContext.Current.Request.UrlReferrer) ?? "");
        }

        /// <summary>
        ///     判断当前页面是否接收到了提交请求
        /// </summary>
        public static bool IsSubmit(SubmitType submitType = SubmitType.Post)
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            var result = false;
            switch (submitType)
            {
                case SubmitType.Get:
                    result = HttpContext.Current.Request.HttpMethod.Equals("GET");
                    break;

                case SubmitType.Post:
                    result = HttpContext.Current.Request.HttpMethod.Equals("POST");
                    break;

                case SubmitType.All:
                    result = IsSubmit(SubmitType.Get) || IsSubmit(SubmitType.Post);
                    break;
            }
            return result;
        }

        /// <summary>
        ///     返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }
            return HttpContext.Current.Request.ServerVariables[strName] ?? string.Empty;
        }

        /// <summary>
        ///     得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }
            var request = HttpContext.Current.Request;
            return !request.Url.IsDefaultPort ? string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString()) : request.Url.Host;
        }

        /// <summary>
        ///     得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        ///     判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            if (HttpContext.Current.Request.UrlReferrer == null)
            {
                return false;
            }
            string[] SearchEngine =
                {
                    "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom",
                    "yisou", "iask", "soso", "gougou", "zhongsou"
                };
            var tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
            return SearchEngine.Any(t => tmpReferrer.IndexOf(t) >= 0);
        }

        /// <summary>
        ///     返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            if (HttpContext.Current == null)
            {
                return 0;
            }
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }

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
        public static T QS<T>(string parmsName, T defValue)
        {
            return HttpContext.Current == null ? defValue : Url.UrlDecode(HttpContext.Current.Request.QueryString[parmsName] ?? string.Empty).ConvertType(defValue);
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
        public static T QS<T>(string parmsName, T defValue, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (HttpContext.Current == null)
            {
                return defValue;
            }
            var requestType = HttpContext.Current.Request.GetType();
            var property = requestType.GetProperty("QueryStringBytes",
                                                            BindingFlags.Instance | BindingFlags.IgnoreCase |
                                                            BindingFlags.NonPublic);
            var queryBytes = (byte[])property.GetValue(HttpContext.Current.Request, null);

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
        ///     Request.Form
        /// </summary>
        public static string QF(string parmsName)
        {
            return QF(parmsName, string.Empty);
        }

        /// <summary>
        ///     Request.Form
        /// </summary>
        public static T QF<T>(string parmsName, T defValue)
        {
            return HttpContext.Current == null ? defValue : HttpContext.Current.Request.Form[parmsName].ConvertType(defValue);
        }

        /// <summary>
        ///     先QF后QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        public static string QA(string parmsName)
        {
            return QA(parmsName, "");
        }

        /// <summary>
        ///     先QF后QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        public static T QA<T>(string parmsName, T defValue)
        {
            return QF(parmsName).IsNullOrEmpty() ? QS(parmsName, defValue) : QF(parmsName, defValue);
        }

        /// <summary>
        ///     获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            var ip = "";
            var context = HttpContext.Current;
            if (context != null)
            {
                ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (ip.IsNullOrEmpty())
                {
                    ip = context.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip.IsNullOrEmpty() || !IsType.IsIP(ip))
                {
                    ip = context.Request.UserHostAddress;
                }
                if (ip.IsNullOrEmpty() || !IsType.IsIP(ip))
                {
                    return "0.0.0.0";
                }
                return ip;
            }
            else
            {
                var strHostName = Dns.GetHostName();
                var ipHost = Dns.GetHostEntry(strHostName);
                foreach (var item in ipHost.AddressList)
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) { ip = item.ToString(); }
                }
            }
            return ip;
        }

        /// <summary>
        ///     判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            var curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            return BrowserName.Any(t => curBrowser.IndexOf(t) >= 0);
        }

        /// <summary>
        ///     判断是否有上传的文件
        /// </summary>
        /// <returns>是否有上传的文件</returns>
        public static bool IsPostFile()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                if (HttpContext.Current.Request.Files[i].FileName != "")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     保存用户上传的文件
        /// </summary>
        /// <param name="path">保存路径</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Files.Count > 0)
            {
                HttpContext.Current.Request.Files[0].SaveAs(path);
            }
        }

        /// <summary>
        ///     Post信息
        /// </summary>
        public static string Post(HttpClient wc, string url, string data, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.Default;
            string strResult = null;
            try
            {
                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                var dataResult = wc.UploadData(url, "POST", encoding.GetBytes(data));
                strResult = encoding.GetString(dataResult);
                wc.Headers.Remove("Content-Type");
            }
            finally
            {
                WriteCookies(wc);
                wc.Dispose();
            }

            return strResult;
        }

        /// <summary>
        ///     获取端口
        /// </summary>
        /// <returns></returns>
        public static int GetPort()
        {
            return HttpContext.Current.Request.Url.Port;
        }

        /// <summary>
        ///     把服務器返回的Cookies信息寫入到客戶端中
        /// </summary>
        public static void WriteCookies(HttpClient wc)
        {
            var setcookie = wc.ResponseHeaders[HttpResponseHeader.SetCookie];
            if (string.IsNullOrEmpty(setcookie)) return;
            var cookie = wc.Headers[HttpRequestHeader.Cookie];
            var cookieList = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(cookie))
            {
                foreach (var ck in cookie.Split(';'))
                {
                    var key = ck.Substring(0, ck.IndexOf('='));
                    var value = ck.Substring(ck.IndexOf('=') + 1);
                    if (!cookieList.ContainsKey(key)) cookieList.Add(key, value);
                }
            }

            foreach (var ck in setcookie.Split(';'))
            {
                var str = ck;
                while (str.Contains(",") && str.IndexOf(',') < str.LastIndexOf('='))
                {
                    str = str.Substring(str.IndexOf(',') + 1);
                }
                var key = str.IndexOf('=') != -1 ? str.Substring(0, str.IndexOf('=')) : "";
                var value = str.Substring(str.IndexOf('=') + 1);
                if (!cookieList.ContainsKey(key))
                    cookieList.Add(key, value);
                else
                    cookieList[key] = value;
            }

            var list = new string[cookieList.Count];
            var index = 0;
            foreach (var pair in cookieList)
            {
                list[index] = string.Format("{0}={1}", pair.Key, pair.Value);
                index++;
            }

            wc.Headers[HttpRequestHeader.Cookie] = list.ToList().ToString(";");
        }

        /// <summary>
        /// 记住用户最后一次访问地址
        /// </summary>
        public static void RememberUrl()
        {
            Cookies.Set(SystemConfigs.ConfigInfo.Cookies_CallBack_Url, Req.GetUrl());
        }

        /// <summary>
        ///     转到网址
        /// </summary>
        public static void GoToUrl(string url, params object[] args)
        {
            if (args != null && args.Length != 0) { url = string.Format(url, args); }
            GoToUrl(url);
        }

        /// <summary>
        ///     转到网址(默认为最后一次访问)
        /// </summary>
        public static void GoToUrl(string url = "")
        {
            if (url.IsNullOrEmpty()) { url = Cookies.Get(SystemConfigs.ConfigInfo.Cookies_CallBack_Url); }
            if (url.IsNullOrEmpty()) { url = "http://" + GetDomain(0); }
            if (url.StartsWith("?")) { url = MvcReq.GetPageName() + url; }
            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        ///     把提交过来的内容转化成为实体类(注意CheckBox 未选中时，是NULL，需要手动判断)
        /// </summary>
        /// <param name="request">NameValueCollection</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事件委托</param>
        /// <param name="url">跳转地址</param>
        public static TInfo Fill<TInfo>(NameValueCollection request, Action<string, string> tip = null, string url = "",
                                        string prefix = "hl") where TInfo : ModelInfo, new()
        {
            if (tip == null) { tip = new Terminator().Alert; }

            Dictionary<string, List<string>> dicError;
            var info = Fill<TInfo>(request, out dicError, prefix);
            if (dicError.Count > 0)
            {
                var lst = new List<string>();
                foreach (var item in dicError)
                {
                    lst.AddRange(item.Value);
                }

                tip(lst.ToString("<br />"), url);
                return null;
            }
            return info;
        }

        /// <summary>
        ///     把提交过来的内容转化成为实体类(注意CheckBox 未选中时，是NULL，需要手动判断)
        /// </summary>
        /// <param name="request">NameValueCollection</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事件委托</param>
        public static TInfo Fill<TInfo>(NameValueCollection request, Action<Dictionary<string, List<string>>> tip,
                                        string prefix = "hl") where TInfo : ModelInfo, new()
        {
            Dictionary<string, List<string>> dicError;
            var info = Fill<TInfo>(request, out dicError, prefix);
            if (dicError.Count > 0)
            {
                tip(dicError);
                return null;
            }
            return info;
        }

        /// <summary>
        ///     把提交过来的内容转化成为实体类(注意CheckBox 未选中时，是NULL，需要手动判断)
        /// </summary>
        /// <param name="request">NameValueCollection</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TInfo Fill<TInfo>(NameValueCollection request, out Dictionary<string, List<string>> dicError, string prefix = "hl") where TInfo : class,IVerification, new()
        {
            dicError = new Dictionary<string, List<string>>();
            var t = new TInfo();
            foreach (var kic in VerifyMapCache.GetMap(t).ModelList)
            {
                var lstError = new List<string>();
                var reqName = prefix + kic.Key.Name;
                if (request[reqName] == null || !kic.Key.CanWrite) { continue; }
                var obj = request[reqName].Trim().ConvertType(kic.Key.PropertyType);
                // 证明类型转换失败
                if (request[reqName].Trim().Length > 0 && obj == null)
                {
                    var type = kic.Key.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }
                    switch (type.Name)
                    {
                        case "DateTime":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为时间格式。如：2011-09-01");
                            break;
                        case "Boolean":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为布尔格式。如：false、true");
                            break;
                        case "UInt64":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为整形格式。");
                            break;
                        case "Int64":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为整形格式。");
                            break;
                        case "Int32":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为整形格式2。");
                            break;
                        case "Int16":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为整形格式。");
                            break;
                        case "Decimal":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为数字格式。");
                            break;
                        case "Byte":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为字节格式。");
                            break;
                        case "Long":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为整形格式。");
                            break;
                        case "Float":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为数字格式。");
                            break;
                        case "Double":
                            lstError.Add("[" + kic.Value.Display.Name + "] 必须为数字格式。");
                            break;
                    }
                }
                if (lstError.Count > 0)
                {
                    dicError.Add(kic.Key.Name, lstError);
                }
                else
                {
                    kic.Key.SetValue(t, obj, null);
                }
            }
            return dicError.Count > 0 ? null : t;
            ;
        }
    }
}