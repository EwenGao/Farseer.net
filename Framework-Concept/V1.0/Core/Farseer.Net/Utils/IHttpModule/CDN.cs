using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using FS.Utils.Common;

namespace FS.Utils.IHttpModule
{
    /// <summary>
    ///     重写模块的抽像基类
    /// </summary>
    /// <remarks></remarks>
    public class CDN : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     加载事件管道
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += CDN_AuthorizeRequest; }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     执行重写功能
        /// </summary>
        protected void CDN_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;

            // 写入追踪日志消息
            app.Context.Trace.Write("CDN加速", "开始执行。");

            var context = (HttpApplication) sender;

            var path = context.Request.RawUrl;
            if (path.Contains("?")) path = path.Substring(0, path.IndexOf('?'));
            var mapPath = context.Server.MapPath(path);

            if (!File.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
                Net.Save(string.Format("http:/{0}", path), mapPath, null);
            }
            app.Context.Trace.Write("CDN加速", "结束执行");
            return;
        }
    }
}