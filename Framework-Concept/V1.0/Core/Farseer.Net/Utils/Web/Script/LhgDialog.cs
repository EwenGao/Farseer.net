using System.Web.UI;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     JS提示框
    /// </summary>
    public class LhgDialog
    {
        private readonly Page page;
        /// <summary>
        /// 对话框脚本
        /// </summary>
        public string DialogScript;

        /// <summary>
        /// Page值
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scriptPre"></param>
        public LhgDialog(Page page, string scriptPre = "frameElement.api.opener.$$")
        {
            this.page = page;
            DialogScript = scriptPre;
        }

        /// <summary>
        ///     Dialog弹出框
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="gotoUrl">跳转页面URL</param>
        public void Alert(string message, string gotoUrl = "")
        {
            if (!gotoUrl.IsNullOrEmpty())
            {
                gotoUrl = "location.href='" + gotoUrl + "'";
            }
            AlertFunc(message, gotoUrl);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.alert弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="func">确定后，执行的脚本</param>
        public void AlertFunc(string message, string func = "")
        {
            var script = DialogScript + ".dialog.alert('" + message + "',function(){ " + func + "; }).zindex().lock();";
            page.Page.ClientScript.RegisterStartupScript(page.GetType(), "System.AlertFunc", script, true);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（可以防止数据保持问题）
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="gotoUrl">跳转地址</param>
        public void Tip(string message, string gotoUrl = "")
        {
            if (!gotoUrl.IsNullOrEmpty())
            {
                gotoUrl = "location.href='" + gotoUrl + "'";
            }
            Tip(message, gotoUrl, 2);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="timeout">自动关闭时间</param>
        /// <param name="func">确定后，执行的脚本</param>
        /// <param name="message">提示内容</param>
        public void Tip(string message, string func, int timeout = 2)
        {
            var script = DialogScript + ".dialog.tips('" + message + "', " + (timeout > 0 ? timeout : 3600) + ", 'tips.gif' ,function(){ " + func + "; }).zindex().lock();";
            page.ClientScript.RegisterStartupScript(page.GetType(), "System.TipFunc", script, true);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="timeout">自动关闭时间</param>
        /// <param name="func">确定后，执行的脚本</param>
        /// <param name="message">提示内容</param>
        public void TipSuccess(string message = "保存成功！", string func = "frameElement.api.close();", int timeout = 1)
        {
            var script = DialogScript + ".dialog.tips('" + message + "', " + (timeout > 0 ? timeout : 3600) + ", 'succ.png' ,function(){ " + func + "; }).zindex().lock();";
            page.ClientScript.RegisterStartupScript(page.GetType(), "System.TipFunc", script, true);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="timeout">自动关闭时间</param>
        /// <param name="func">确定后，执行的脚本</param>
        /// <param name="message">提示内容</param>
        public void TipError(string message = "数据不存在！", string func = "frameElement.api.close();", int timeout = 2)
        {
            var script = DialogScript + ".dialog.tips('" + message + "', " + (timeout > 0 ? timeout : 3600) + ", 'fail.png' ,function(){ " + func + "; }).zindex().lock();";
            page.ClientScript.RegisterStartupScript(page.GetType(), "System.TipFunc", script, true);
        }
    }
}