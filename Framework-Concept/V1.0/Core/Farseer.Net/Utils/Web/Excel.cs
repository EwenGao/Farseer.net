using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using FS.Utils.Common;

namespace FS.Utils.Web
{
    /// <summary>
    ///     Microsoft.Excel
    /// </summary>
    public abstract class Excel
    {
        /// <summary>
        ///     生成Excel文件
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="fileName">保存的文件名</param>
        public static void CreateExcel(DataTable dt, string fileName)
        {
            var response = HttpContext.Current.Response;

            response.ContentEncoding = Encoding.GetEncoding("gb2312");
            response.AppendHeader("content-disposition",
                                  "attachment;filename=\"" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + ".xls\"");
            response.ContentType = "application/ms-excel";

            var SP = new StrPlus();
            SP.Append("<table border='1' cellspacing='0' cellpadding='0'>");

            SP.Append("<tr>");
            foreach (DataColumn item in dt.Columns)
            {
                SP.AppendFormat(
                    "<td style='font-size: 12px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='20'>{0}</td>",
                    item.Caption);
            }
            SP.Append("</tr>");


            //定义表对象与行对象，同时用DataSet对其值进行初始化 
            foreach (DataRow row in dt.Rows)
            {
                SP.Append("<tr>");

                //在当前行中，逐列获得数据，数据之间以\t分割，结束时加回车符\n 
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    SP.AppendFormat("<td style='background-color: #E9ECED;font-size: 12px;'>{0}</td>",
                                          row[i].ToString());
                }
                SP.Append("</tr>");
            }

            SP.Append("</table>");
            response.Write(SP);
            response.End();
        }

        /// <summary>
        ///     datagrid生成
        /// </summary>
        /// <param name="ctl"></param>
        public static void ToExcel(Control ctl)
        {
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=Excel.xls");
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword 
            ctl.Page.EnableViewState = false;
            var tw = new StringWriter();
            var hw = new HtmlTextWriter(tw);
            ctl.RenderControl(hw);
            HttpContext.Current.Response.Write(tw.ToString());
            HttpContext.Current.Response.End();
        }
    }
}