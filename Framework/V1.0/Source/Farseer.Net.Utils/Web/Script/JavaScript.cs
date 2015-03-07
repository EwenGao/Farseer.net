using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     原生的脚本
    /// </summary>
    public class JavaScript
    {
        private readonly Page page;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="page"></param>
        public JavaScript(Page page)
        {
            this.page = page;
        }

        /// <summary>
        ///     输出指定的提示信息
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="title">标题</param>
        /// <param name="links">链拉 例：是,yes.htm|否,no.htm</param>
        /// <param name="url">跳转页面URL</param>
        /// <param name="showback">是否显示返回链接</param>
        public void Message(string message, string title = "", string links = "", string url = "", bool showback = true)
        {
            new Terminator().Throw(message, title, links, url, showback);
        }

        /// <summary>
        ///     alert弹出框
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="gotoUrl">跳转页面URL</param>
        public void Alert(string message, string gotoUrl = "")
        {
            new Terminator().Alert(message, gotoUrl);
        }

        /// <summary>
        ///     使用alert弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="func">确定后，执行的脚本</param>
        public void AlertFunc(string message, string func = "")
        {
            var script = "alert('" + message + "');" + func + ";";
            page.Page.ClientScript.RegisterStartupScript(page.GetType(), "System.AlertFunc", script, true);
        }

        /// <summary>
        ///     出错时，输出TipError的脚本方法。用于自定义提示错误
        /// </summary>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public void ScriptError(Dictionary<string, List<string>> dicError)
        {
            var lst = dicError.Select(item => string.Format(@"TipError(""{0}"",""[""{1}""]"");", item.Key, item.Value.ToString("\",\""))).ToList();
            page.ClientScript.RegisterStartupScript(page.GetType(), "System.TipError", lst.ToString(" "), true);
        }
    }
}