using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     解释Html
    /// </summary>
    public abstract class Htmls
    {
        /// <summary>
        ///     正则
        /// </summary>
        public static Regex RegexFont = new Regex(@"<font color=" + "\".*?\"" + @">([\s\S]+?)</font>", RegexOptions.None);

        /// <summary>
        ///     返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            if (str.IsNullOrEmpty()) { return string.Empty; }
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        ///     返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            if (str.IsNullOrEmpty()) { return string.Empty; }
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        ///     移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            if (content.IsNullOrEmpty()) { return string.Empty; }
            return Regex.Replace(content, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            if (content.IsNullOrEmpty()) { return string.Empty; }
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2",
                                    RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        ///     从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="html"></param>
        public static string GetInnerText(string html)
        {
            if (html.IsNullOrEmpty()) { return string.Empty; }
            var regEx = new Regex(@"</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase);

            return regEx.Replace(html, "");
        }

        /// <summary>
        ///     生成指定数量的html空格符号
        /// </summary>
        public static string Spaces(int nSpaces)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < nSpaces; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.ToString();
        }

        /// <summary>
        ///     检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }

        /// <summary>
        ///     替换回车换行符为html换行符
        /// </summary>
        public static string CodeToHtml(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            str = str.Replace("\r\n", "<br />");
            str = str.Replace("\n", "<br />");
            return str;
        }

        /// <summary>
        ///     替换html换行符为回车换行符
        /// </summary>
        public static string HtmlToCode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            str = str.Replace("<br />", "\r\n");
            str = str.Replace("<br/>", "\r\n");
            str = str.Replace("<br>", "\r\n");
            return str;
        }

        /// <summary>
        ///     Email 编码
        /// </summary>
        /// <param name="html">要编码的字符串</param>
        public static string EmailEncode(string html)
        {
            var email = (HtmlEncode(html)).Replace("@", "&#64;").Replace(".", "&#46;");
            return string.Format("<a href='mailto:{0}'>{0}</a>", email);
        }

        /// <summary>
        ///     JavaScript 编码
        /// </summary>
        /// <param name="script">要编码的字符串</param>
        public static string ScriptEncode(string script)
        {
            var sb = new StringBuilder(script);
            sb.Replace("\\", "\\\\");
            sb.Replace("\r", "\\0Dx");
            sb.Replace("\n", "\\x0A");
            sb.Replace("\"", "\\x22");
            sb.Replace("\'", "\\x27");
            return sb.ToString();
        }

        /// <summary>
        ///     过滤脏词，默认有为“妈的|你妈|他妈|妈b|妈比|fuck|shit|我日|法轮|我操”
        /// </summary>
        /// <param name="text">要编码的字符串</param>
        public static string ShitEncode(string text)
        {
            //string bw = Config.Settings["BadWords"];
            var bw = "";
            if (bw.IsNullOrEmpty())
            {
                bw = "妈的|你妈|他妈|妈b|妈比|fuck|shit|我日|法轮|我操";
            }
            else
            {
                bw = Regex.Replace(bw, @"\|{2,}", "|");
                bw = Regex.Replace(bw, @"(^\|)|(\|$)", string.Empty);
            }
            return Regex.Replace(text, bw, "**", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 压缩html
        /// </summary>
        /// <param name="html"></param>
        public static string Compression(string html)
        {
            html = html.ClearString(Regex.Escape("\r\n")).ClearString(Regex.Escape("\r"));
            html = Regex.Replace(html, ">[\\s\\r\\n]*<", "><");
            return Regex.Replace(html, "\\s+", " ");
        }
    }
}