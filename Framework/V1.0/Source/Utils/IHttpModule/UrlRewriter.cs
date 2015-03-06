using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.Adapters;
using FS.Configs;
using FS.Extend;
using FS.Utils.Web;

namespace FS.Utils.IHttpModule
{
    /// <summary>
    ///     重写模块的抽像基类
    /// </summary>
    /// <remarks></remarks>
    public class UrlRewriter : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     加载事件管道
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += UrlRewriter_AuthorizeRequest; }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     执行重写功能
        /// </summary>
        protected void UrlRewriter_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            var appPath = app.Context.Request.ApplicationPath;

            // 判断是否已重写
            if (app.Request.Url.AbsoluteUri.IndexOf("UrlRewriter=true") > 0) { return; }

            // 写入追踪日志消息
            app.Context.Trace.Write("Url重写", "开始执行。");


            // 循环所有重写规则
            foreach (var rule in RewriterConfigs.ConfigInfo.Rules)
            {
                // 取得规则地址
                var lookFor = "^" + ResolveUrl(appPath, rule.LookFor) + "$";
                var re = new Regex(lookFor, RegexOptions.IgnoreCase);
                var url = lookFor.IsStartsWith("^http://") ? app.Request.Url.AbsoluteUri : app.Request.Path;

                if (!re.IsMatch(url)) continue;

                // 取得重写规则地址
                var sendToUrl = ResolveUrl(appPath, re.Replace(url, rule.SendTo));
                // 写入追踪日志消息
                app.Context.Trace.Write("Url重写", "重写到：" + sendToUrl);
                // 重写地址
                RewriteUrl(app.Context, sendToUrl);
                break;
            }
            app.Context.Trace.Write("Url重写", "结束执行");
            return;
        }

        /// <summary>
        ///     替换多个域、并且将本地路径转换成网站路径
        /// </summary>
        /// <param name="appPath">网站根目录.</param>
        /// <param name="url">规则地址</param>
        protected string ResolveUrl(string appPath, string url)
        {
            //替换多个域
            url = string.Format(url, GeneralConfigs.ConfigInfo.RewriterDomain.ToArray("", ";"));
            if (url.Length == 0 || url[0] != '~') { return url; }
            // 如果不是使用目录地址，则直接返回原地址
            if (url.Length == 1) { return appPath; }

            // 返回根目录地址
            return appPath + Url.ConvertPath(url[1] == '/' ? url.Substring(2) : url.Substring(1));
        }

        /// <summary>
        ///     重写地址
        /// </summary>
        /// <param name="context">请求的上下文</param>
        /// <param name="sendToUrl">重写的目的URL</param>
        protected void RewriteUrl(HttpContext context, string sendToUrl)
        {
            string x, y;
            RewriteUrl(context, sendToUrl, out x, out y);
        }

        /// <summary>
        ///     重写地址
        /// </summary>
        /// <param name="context">请求的上下文</param>
        /// <param name="sendToUrl">重写的目的URL</param>
        /// <param name="filePath">重写目的URL的实际物理路径</param>
        /// <param name="sendToUrlLessQString">不能参数的地址</param>
        protected void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath)
        {
            sendToUrlLessQString = sendToUrl.SubString(0, sendToUrl.IndexOf('?'));
            var queryString = String.Empty;
            if (sendToUrl.IndexOf('?') > -1) { queryString = sendToUrl.SubString(sendToUrl.IndexOf('?') + 1); }
            if (queryString.Length > 1 && !queryString.EndsWith("&")) { queryString += "&"; }
            queryString += "UrlRewriter=true";

            // grab the file's physical path
            filePath = string.Empty;
            //filePath = context.Server.MapPath(sendToUrlLessQString);

            // rewrite the path...
            context.RewritePath(sendToUrlLessQString, String.Empty, queryString);
        }
    }

    /// <summary>
    ///     重写FORM的Action地址
    /// </summary>
    public class FormRewriterControlAdapter : ControlAdapter
    {
        /// <summary>
        ///     重写HTML输出
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(new RewriteFormHtmlTextWriter(writer));
        }
    }

    /// <summary>
    ///     Form表单
    /// </summary>
    public class RewriteFormHtmlTextWriter : HtmlTextWriter
    {
        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public RewriteFormHtmlTextWriter(HtmlTextWriter writer)
            : base(writer)
        {
            base.InnerWriter = writer.InnerWriter;
        }

        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public RewriteFormHtmlTextWriter(TextWriter writer)
            : base(writer)
        {
            base.InnerWriter = writer;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="fEncode"></param>
        public override void WriteAttribute(string name, string value, bool fEncode)
        {
            //If the attribute we are writing is the "action" attribute, and we are not on a sub-control, 
            //then replace the value to write with the raw URL of the request - which ensures that we'll
            //preserve the PathInfo value on postback scenarios
            if (name == "action")
            {
                var context = HttpContext.Current;
                if (context.Items["ActionAlreadyWritten"] == null)
                {
                    //We will use the Request.RawUrl property within ASP.NET to retrieve the origional 
                    //URL before it was re-written.
                    value = context.Request.RawUrl;
                    //Indicate that we've already rewritten the <form>'s action attribute to prevent
                    //us from rewriting a sub-control under the <form> control
                    context.Items["ActionAlreadyWritten"] = true;
                }
            }
            base.WriteAttribute(name, value, fEncode);
        }
    }
}