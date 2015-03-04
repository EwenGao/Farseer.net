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
    ///     Request������
    /// </summary>
    public abstract class Req
    {
        /// <summary>
        ///     ��������
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
            ///     Get �� Post (Post����)
            /// </summary>
            All = 255
        }

        /// <summary>
        ///     ��ȡ��ǰӦ�ó�����ʵ�����·����Ϣ
        ///     http://localhost:1480/WebSite2/Default.aspx?UserID=1
        /// </summary>
        /// <returns></returns>
        public static string GetUrl()
        {
            return HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        ///     ��ȡ��ǰӦ�ó�����ʵ�����(���˿�)
        ///     www.xxx.com:81
        /// </summary>
        /// <param name="node">Ҫ��ȡ�ڼ����ڵ㣬0��������</param>
        public static string GetDomain(int node = 3)
        {
            var domain = HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.Url.Authority;
            if (node < 1) { return domain; }

            var lstDomain = domain.ToList("", ".");
            while (lstDomain.Count > node) { lstDomain.RemoveAt(0); }
            return lstDomain.ToString(".");
        }

        /// <summary>
        ///     ��ȡ��ǰӦ�ó�����ʵ�·��(��������������ҳ���ļ�����URL��д��ַ����ʵ·��)
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
        /// ��ȡ��ǰӦ�ó�����ʵ�·��(��������������ҳ���ļ���)
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
        ///     ��ȡ��ǰӦ�ó�����ʵ�ҳ���ļ�����(��������)
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
        ///     ��ȡ��ǰӦ�ó�����ʵĲ���
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
        /// �����ɲ����ַ���
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
        ///     ��һ��ҳ���ַ
        /// </summary>
        public static string GetPrevious()
        {
            return HttpContext.Current == null ? string.Empty : Convert.ToString(((object)HttpContext.Current.Request.UrlReferrer) ?? "");
        }

        /// <summary>
        ///     �жϵ�ǰҳ���Ƿ���յ����ύ����
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
        ///     ����ָ���ķ�����������Ϣ
        /// </summary>
        /// <param name="strName">������������</param>
        /// <returns>������������Ϣ</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }
            return HttpContext.Current.Request.ServerVariables[strName] ?? string.Empty;
        }

        /// <summary>
        ///     �õ���ǰ��������ͷ
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
        ///     �õ�����ͷ
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        ///     �ж��Ƿ�����������������
        /// </summary>
        /// <returns>�Ƿ�����������������</returns>
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
        ///     ���ر���Url�������ܸ���
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
        ///     ��QF��QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        public static string QA(string parmsName)
        {
            return QA(parmsName, "");
        }

        /// <summary>
        ///     ��QF��QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        public static T QA<T>(string parmsName, T defValue)
        {
            return QF(parmsName).IsNullOrEmpty() ? QS(parmsName, defValue) : QF(parmsName, defValue);
        }

        /// <summary>
        ///     ��õ�ǰҳ��ͻ��˵�IP
        /// </summary>
        /// <returns>��ǰҳ��ͻ��˵�IP</returns>
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
        ///     �жϵ�ǰ�����Ƿ�������������
        /// </summary>
        /// <returns>��ǰ�����Ƿ�������������</returns>
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
        ///     �ж��Ƿ����ϴ����ļ�
        /// </summary>
        /// <returns>�Ƿ����ϴ����ļ�</returns>
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
        ///     �����û��ϴ����ļ�
        /// </summary>
        /// <param name="path">����·��</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Files.Count > 0)
            {
                HttpContext.Current.Request.Files[0].SaveAs(path);
            }
        }

        /// <summary>
        ///     Post��Ϣ
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
        ///     ��ȡ�˿�
        /// </summary>
        /// <returns></returns>
        public static int GetPort()
        {
            return HttpContext.Current.Request.Url.Port;
        }

        /// <summary>
        ///     �ѷ��������ص�Cookies��Ϣ���뵽�͑�����
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
        /// ��ס�û����һ�η��ʵ�ַ
        /// </summary>
        public static void RememberUrl()
        {
            Cookies.Set(SystemConfigs.ConfigInfo.Cookies_CallBack_Url, Req.GetUrl());
        }

        /// <summary>
        ///     ת����ַ
        /// </summary>
        public static void GoToUrl(string url, params object[] args)
        {
            if (args != null && args.Length != 0) { url = string.Format(url, args); }
            GoToUrl(url);
        }

        /// <summary>
        ///     ת����ַ(Ĭ��Ϊ���һ�η���)
        /// </summary>
        public static void GoToUrl(string url = "")
        {
            if (url.IsNullOrEmpty()) { url = Cookies.Get(SystemConfigs.ConfigInfo.Cookies_CallBack_Url); }
            if (url.IsNullOrEmpty()) { url = "http://" + GetDomain(0); }
            if (url.StartsWith("?")) { url = MvcReq.GetPageName() + url; }
            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        ///     ���ύ����������ת����Ϊʵ����(ע��CheckBox δѡ��ʱ����NULL����Ҫ�ֶ��ж�)
        /// </summary>
        /// <param name="request">NameValueCollection</param>
        /// <param name="prefix">�ؼ�ǰ׺</param>
        /// <param name="tip">�������¼�ί��</param>
        /// <param name="url">��ת��ַ</param>
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
        ///     ���ύ����������ת����Ϊʵ����(ע��CheckBox δѡ��ʱ����NULL����Ҫ�ֶ��ж�)
        /// </summary>
        /// <param name="request">NameValueCollection</param>
        /// <param name="prefix">�ؼ�ǰ׺</param>
        /// <param name="tip">�������¼�ί��</param>
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
        ///     ���ύ����������ת����Ϊʵ����(ע��CheckBox δѡ��ʱ����NULL����Ҫ�ֶ��ж�)
        /// </summary>
        /// <param name="request">NameValueCollection</param>
        /// <param name="prefix">�ؼ�ǰ׺</param>
        /// <param name="dicError">���ش�����Ϣ,key���������ƣ�value��������Ϣ</param>
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
                // ֤������ת��ʧ��
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
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊʱ���ʽ���磺2011-09-01");
                            break;
                        case "Boolean":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ������ʽ���磺false��true");
                            break;
                        case "UInt64":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���θ�ʽ��");
                            break;
                        case "Int64":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���θ�ʽ��");
                            break;
                        case "Int32":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���θ�ʽ2��");
                            break;
                        case "Int16":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���θ�ʽ��");
                            break;
                        case "Decimal":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���ָ�ʽ��");
                            break;
                        case "Byte":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ�ֽڸ�ʽ��");
                            break;
                        case "Long":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���θ�ʽ��");
                            break;
                        case "Float":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���ָ�ʽ��");
                            break;
                        case "Double":
                            lstError.Add("[" + kic.Value.Display.Name + "] ����Ϊ���ָ�ʽ��");
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