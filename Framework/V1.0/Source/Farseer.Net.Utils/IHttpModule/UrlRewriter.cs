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
    ///     ��дģ��ĳ������
    /// </summary>
    /// <remarks></remarks>
    public class UrlRewriter : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     �����¼��ܵ�
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += UrlRewriter_AuthorizeRequest; }

        /// <summary>
        ///     ע��
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     ִ����д����
        /// </summary>
        protected void UrlRewriter_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            var appPath = app.Context.Request.ApplicationPath;

            // �ж��Ƿ�����д
            if (app.Request.Url.AbsoluteUri.IndexOf("UrlRewriter=true") > 0) { return; }

            // д��׷����־��Ϣ
            app.Context.Trace.Write("Url��д", "��ʼִ�С�");


            // ѭ��������д����
            foreach (var rule in RewriterConfigs.ConfigInfo.Rules)
            {
                // ȡ�ù����ַ
                var lookFor = "^" + ResolveUrl(appPath, rule.LookFor) + "$";
                var re = new Regex(lookFor, RegexOptions.IgnoreCase);
                var url = lookFor.IsStartsWith("^http://") ? app.Request.Url.AbsoluteUri : app.Request.Path;

                if (!re.IsMatch(url)) continue;

                // ȡ����д�����ַ
                var sendToUrl = ResolveUrl(appPath, re.Replace(url, rule.SendTo));
                // д��׷����־��Ϣ
                app.Context.Trace.Write("Url��д", "��д����" + sendToUrl);
                // ��д��ַ
                RewriteUrl(app.Context, sendToUrl);
                break;
            }
            app.Context.Trace.Write("Url��д", "����ִ��");
            return;
        }

        /// <summary>
        ///     �滻����򡢲��ҽ�����·��ת������վ·��
        /// </summary>
        /// <param name="appPath">��վ��Ŀ¼.</param>
        /// <param name="url">�����ַ</param>
        protected string ResolveUrl(string appPath, string url)
        {
            //�滻�����
            url = string.Format(url, GeneralConfigs.ConfigInfo.RewriterDomain.ToArray("", ";"));
            if (url.Length == 0 || url[0] != '~') { return url; }
            // �������ʹ��Ŀ¼��ַ����ֱ�ӷ���ԭ��ַ
            if (url.Length == 1) { return appPath; }

            // ���ظ�Ŀ¼��ַ
            return appPath + Url.ConvertPath(url[1] == '/' ? url.Substring(2) : url.Substring(1));
        }

        /// <summary>
        ///     ��д��ַ
        /// </summary>
        /// <param name="context">�����������</param>
        /// <param name="sendToUrl">��д��Ŀ��URL</param>
        protected void RewriteUrl(HttpContext context, string sendToUrl)
        {
            string x, y;
            RewriteUrl(context, sendToUrl, out x, out y);
        }

        /// <summary>
        ///     ��д��ַ
        /// </summary>
        /// <param name="context">�����������</param>
        /// <param name="sendToUrl">��д��Ŀ��URL</param>
        /// <param name="filePath">��дĿ��URL��ʵ������·��</param>
        /// <param name="sendToUrlLessQString">���ܲ����ĵ�ַ</param>
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
    ///     ��дFORM��Action��ַ
    /// </summary>
    public class FormRewriterControlAdapter : ControlAdapter
    {
        /// <summary>
        ///     ��дHTML���
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(new RewriteFormHtmlTextWriter(writer));
        }
    }

    /// <summary>
    ///     Form��
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