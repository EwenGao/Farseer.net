using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using FS.Utils.Common;

namespace FS.Utils.IHttpModule
{
    /// <summary>
    ///     ��дģ��ĳ������
    /// </summary>
    /// <remarks></remarks>
    public class CDN : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     �����¼��ܵ�
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += CDN_AuthorizeRequest; }

        /// <summary>
        ///     ע��
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     ִ����д����
        /// </summary>
        protected void CDN_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;

            // д��׷����־��Ϣ
            app.Context.Trace.Write("CDN����", "��ʼִ�С�");

            var context = (HttpApplication) sender;

            var path = context.Request.RawUrl;
            if (path.Contains("?")) path = path.Substring(0, path.IndexOf('?'));
            var mapPath = context.Server.MapPath(path);

            if (!File.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
                Net.Save(string.Format("http:/{0}", path), mapPath, null);
            }
            app.Context.Trace.Write("CDN����", "����ִ��");
            return;
        }
    }
}