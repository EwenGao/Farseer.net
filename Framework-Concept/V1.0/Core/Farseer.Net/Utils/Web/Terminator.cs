using System.Text;
using System.Web;
using System.Web.UI;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     程序终止调度
    /// </summary>
    public class Terminator
    {
        /// <summary>
        ///     页面终止页面模板
        /// </summary>
        public virtual string template
        {
            get { return @"<html xmlns:v>
				<head>
				<title>{$Title}</title>
				<meta http-equiv='Content-Type' content='text/html; charset=" + Encoding.Default.BodyName + @"' />
				<meta name='description' content='.NET FS.Web.dll Page Terminator Code' />
				<meta name='copyright' content='http://www.Qyn99.net/' />
				<meta name='generator' content='vs2008' />
				<meta name='usefor' content='application termination' />
				{$AutoJump}
				<style rel='stylesheet'>
				v\:*	{
					behavior:url(#default#vml);
				}
				body, div, span, li, td, a {
					color: #222222;
					font-size: 12px !important;
					font-size: 11px;
					font-family: tahoma, arial, 'courier new', verdana, sans-serif;
					line-height: 19px;
				}
				a {
					color: #2c78c5;
					text-decoration: none;
				}
				a:hover {
					color: red;
					text-decoration: none;
				}
				</style>
				</head>
				<body style='text-align:center;margin:90px 20px 50px 20px'>
				<?xml:namespace prefix='v' />
				<div style='margin:auto; width:450px; text-align:center'>
					<v:roundrect style='text-align:left; display:table; margin:auto; padding:15px; width:450px; height:210px; overflow:hidden; position:relative;' arcsize='3200f' coordsize='21600,21600' fillcolor='#fdfdfd' strokecolor='#e6e6e6' strokeweight='1px'>
						<table width='100%' cellpadding='0' cellspacing='0' border='0' style='padding-bottom:6px; border-bottom:1px #cccccc solid'>
							<tr>
								<td><b>{$Title}</b></td>
								<td align='right' style='color:#f8f8f8'>--- Qyn terminator</td>
							</tr>
						</table>
						<table width='100%' cellpadding='0' cellspacing='0' border='0' style='word-break:break-all; overflow:hidden'>
							<tr>
								<td width='80' valign='top' style='padding-top:13px'><span style='font-size:16px; zoom:4; color:#aaaaaa'><font face='webdings'>i</font></span></td>
								<td valign='top' style='padding-top:17px'>
									<p style='margin-bottom:22px'>{$Message}</p>
									{$Links}
								</td>
							</tr>
						</table>
					</v:roundrect>
				</div>
				</body>
				</html>"; }
        }

        private void Echo(string s)
        {
            HttpContext.Current.Response.Write(s);
        }

        /// <summary>
        ///     end
        /// </summary>
        private void End()
        {
            HttpContext.Current.Response.End();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        ///     JS弹出框
        /// </summary>
        /// <param name="message">提示内容</param>
        public virtual void Alert(string message)
        {
            Alert(message, "");
        }

        /// <summary>
        ///     JS弹出框
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="gotoUrl">跳转页面URL</param>
        public virtual void Alert(string message, string gotoUrl = "")
        {
            if (gotoUrl.IsNullOrEmpty())
            {
                gotoUrl = string.Empty;
            }
            if (gotoUrl.StartsWith("?"))
            {
                gotoUrl = Req.GetPageName() + gotoUrl;
            }
            gotoUrl = gotoUrl.IsNullOrEmpty() ? "window.history.back();" : "location.href='" + gotoUrl + "';";
            Echo("<script language='javascript'>alert('" + message.Replace("'", @"\'").Replace("<br />", "\\r") + "');" +
                 gotoUrl + "</script>");
            End();
        }

        /// <summary>
        ///     alert javascript
        /// </summary>
        /// <param name="p">页面</param>
        /// <param name="message">警告内容</param>
        public virtual void Alert(Page p, string message)
        {
            var scriptKey = "AlertKey";
            var scripText = "alert('" + message.Replace("'", @"\'").Replace("<br />", "\\r") + "');";
            var csm = p.ClientScript;
            var t = p.GetType();
            if (!csm.IsClientScriptBlockRegistered(scriptKey))
            {
                csm.RegisterClientScriptBlock(t, scriptKey, scripText, true);
            }
        }

        /// <summary>
        ///     输出指定的提示信息
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="title">标题</param>
        /// <param name="links">链拉 例：是,yes.htm|否,no.htm</param>
        /// <param name="url">跳转页面URL</param>
        /// <param name="showback">是否显示返回链接</param>
        public virtual void Throw(string message, string title = "", string links = "", string url = "",
                                  bool showback = true)
        {
            HttpContext.Current.Response.ContentType = "text/html";

            if (!links.IsNullOrEmpty() && links.StartsWith("?"))
            {
                links = Req.GetPageName() + links;
            }

            var sb = new StringBuilder(template);

            sb.Replace("{$Message}", message);
            sb.Replace("{$Title}", title.IsNullOrEmpty() ? "System Tips" : title);

            if (!links.IsNullOrEmpty())
            {
                var arr1 = links.Split('|');
                foreach (var str in arr1)
                {
                    var arr2 = str.Split(',');
                    if (arr2.Length <= 1) continue;
                    if (arr2[1].Trim() == "RefHref")
                    {
                        arr2[1] = Req.GetPrevious();
                        arr2[1] = Req.GetUrl();
                    }
                    if (arr2[1].IsNullOrEmpty())
                    {
                        continue;
                    }

                    var s = ("<li><a href='" + arr2[1] + "'");
                    if (arr2.Length == 3)
                    {
                        s += (" target='" + arr2[2].Trim() + "'");
                    }

                    if (arr2[0].Trim() == "RefText")
                    {
                        arr2[0] = Htmls.HtmlEncode(Req.GetPrevious());
                    }

                    s += (">" + arr2[0].Trim() + "</a></li>\r\n\t\t\t\t");
                    sb.Replace("{$Links}", s + "{$Links}");
                }
            }

            if (!url.IsNullOrEmpty())
            {
                var s = url == "back" ? "javascript:history.back()" : url;
                sb.Replace("{$AutoJump}", "<meta http-equiv='refresh' content='3; url=" + s + "' />");
            }
            else
            {
                sb.Replace("{$AutoJump}", "<!-- no jump -->");
            }

            sb.Replace("{$Links}",
                       showback ? "<li><a href='javascript:history.back()'>Back Page</a></li>" : "<!-- no back -->");

            Echo(sb.ToString());
            End();
        }
    }
}