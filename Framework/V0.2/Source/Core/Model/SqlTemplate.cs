using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FS.Configs;
using FS.Core.Bean;
using FS.Extend;
using FS.Utils.Common;

namespace FS.Core.Model
{
    internal class SqlTemplate
    {
        private static object objLock = new object();

        /// <summary>
        ///     输出执行结果
        /// </summary>
        internal static void Output<TInfo>(DataResult<TInfo> DataResult) where TInfo : ModelInfo, new()
        {
            if (!SystemConfigs.ConfigInfo.IsWriteDbLog)
            {
                return;
            }
            var fileName = "";

            var str = new StringBuilder();
            str.Append("<script>insert(");
            str.Append(string.Format("\"{0}\"", DateTime.Now.ToLongString("HH:mm")));
            str.Append(string.Format(",\"{0}\"", DataResult.SqlBuildTime));
            str.Append(string.Format(",\"{0}\"", DataResult.OperateDataTime));
            str.Append(string.Format(",\"{0}\"", DataResult.ToModelTime));

            var stack = new StackTrace(true).GetFrames().LastOrDefault(o => o.GetFileLineNumber() != 0
                                                                            &&
                                                                            !o.GetMethod().Module.Name.IsEquals("FS.dll")
                                                                            && !o.GetMethod().Name.IsEquals("Callback"));

            if (stack != null)
            {
                fileName = stack.GetFileName().SubString(Files.GetRootPath().Length);
                str.Append(string.Format(",\"{0}\"", fileName));
                str.Append(string.Format(",\"{0}\"", stack.GetFileLineNumber()));
                str.Append(string.Format(",\"{0}\"", stack.GetMethod().Name));
            }
            else
            {
                str.Append(",\"\"");
                str.Append(",\"\"");
                str.Append(",\"\"");
            }

            var sql = DataResult.Sql;
            foreach (var item in DataResult.ParmsList)
            {
                switch (item.DbType)
                {
                    case System.Data.DbType.Date:
                    case System.Data.DbType.DateTime:
                    case System.Data.DbType.DateTime2:
                    case System.Data.DbType.DateTimeOffset:
                    case System.Data.DbType.String:
                    case System.Data.DbType.StringFixedLength:
                    case System.Data.DbType.Time:
                    case System.Data.DbType.Xml:
                        sql = sql.Replace(item.ParameterName + ")", "'" + item.Value.ToString() + "')");
                        break;
                    case System.Data.DbType.AnsiString:
                    case System.Data.DbType.AnsiStringFixedLength:
                    case System.Data.DbType.Binary:
                    case System.Data.DbType.Boolean:
                    case System.Data.DbType.Byte:
                    case System.Data.DbType.Currency:
                    case System.Data.DbType.Decimal:
                    case System.Data.DbType.Double:
                    case System.Data.DbType.Guid:
                    case System.Data.DbType.Int16:
                    case System.Data.DbType.Int32:
                    case System.Data.DbType.Int64:
                    case System.Data.DbType.Object:
                    case System.Data.DbType.SByte:
                    case System.Data.DbType.Single:
                    case System.Data.DbType.UInt16:
                    case System.Data.DbType.UInt32:
                    case System.Data.DbType.UInt64:
                    case System.Data.DbType.VarNumeric:
                    default:
                        sql = sql.Replace(item.ParameterName + ")", item.Value.ToString() + ")");
                        break;
                }
            }
            str.Append(string.Format(",\"{0}\"", sql));

            str.Append(");</script>");
            try
            {
                var path = Files.GetRootPath() + "App_Data/Sql/" + DateTime.Now.ToShortString() + "/" + fileName +
                              ".html";
                CreateHtml(path);

                File.AppendAllText(path, str.ToString(), Encoding.UTF8);
            }
            catch
            {
            }
        }

        private static void CreateHtml(string path)
        {
            if (File.Exists(path))
            {
                return;
            }
            var html =
                @"<html xmlns='http://www.w3.org/1999/xhtml'><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><title>default</title></head>
                        <body>
                        <table id='table' width='100%' border='1' cellspacing='0' cellpadding='0' bordercolor='#CCCCCC' style='margin: 0 auto; font-size: 12px; text-align: center; line-height: 25px; border-collapse: collapse;'>
                             <tr style='font-weight: bold; background: #f3f3f3;'>
                                 <td width='40'>行号</td>
                                <td width='60'>时间</td>
                                <td width='100'>生成</td>
                                <td width='100'>获取</td>
                                <td width='100'>转换</td>
                                <td>路径</td>
                                <td width='40'>行数</td>
                                <td width='250'>方法</td>
                            </tr>
                        </table>
                    </body>
                    <script>
                        var Index = 0;
                        function insert(time, sqlBuildTime, operateDataTime, toModelTime, path, lineNum, methodName, sql) {
                            var table = document.getElementById('table');
                            table.innerHTML += ""<tr>""
                                                + ""<td rowspan='2'>""+ ++Index +""</td>""
                                                + ""<td>["" + time + ""]</td>""
                                                + ""<td>"" + sqlBuildTime + ""</td>""
                                                + ""<td>"" + operateDataTime + ""</td>""
                                                + ""<td>"" + toModelTime + ""</td>""
                                                + ""<td>"" + path + ""</td>""
                                                + ""<td>"" + lineNum + ""</td>""
                                                + ""<td>"" + methodName + ""</td>""
                                            + ""</tr>""
                                            + ""<tr>""
                                                + ""<td width='60' style='font-weight: bold; background: #f3f3f3;'>SQL</td>""
                                                + ""<td colspan='6' style='text-align: left; padding: 0 10px;'>"" + sql + ""</td>""
                                            + ""</tr>""
                                            + ""<tr style='height: 25px;'><td colspan='8'></td></tr>"";
                        }
                    </script>
                </html>
                ";

            Directory.CreateDirectory(path);
            File.WriteAllText(path, html, Encoding.UTF8);
        }
    }
}