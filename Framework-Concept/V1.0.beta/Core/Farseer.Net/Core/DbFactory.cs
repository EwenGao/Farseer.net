using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using FS.Configs;
using FS.Core.Data;

namespace Farseer.Net.Core
{
    public class DbFactory
    {
        /// <summary>
        /// 返回数据库类型名称
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string GetDbTypeName(DataBaseType dbType)
        {
            var dbTypeName = string.Empty;
            switch (dbType)
            {
                case DataBaseType.MySql: dbTypeName = "MySql.Data.MySqlClient"; break;
                case DataBaseType.OleDb: dbTypeName = "System.Data.OleDb"; break;
                case DataBaseType.Oracle: dbTypeName = "System.Data.OracleClient"; break;
                case DataBaseType.SQLite: dbTypeName = "System.Data.SQLite"; break;
                case DataBaseType.SqlServer: dbTypeName = "System.Data.SqlClient"; break;
                case DataBaseType.Xml: dbTypeName = "System.Linq.Xml"; break;
            }
            return dbTypeName;
        }

        /// <summary>
        /// 获取数据库连接对象
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        public static DbConnection GetDbConnection(DataBaseType dbType, string connectionString)
        {
            DbConnection conn;
            switch (dbType)
            {
                case DataBaseType.SqlServer: conn = new SqlConnection(connectionString); ; break;
                default: conn = DbProviderFactories.GetFactory(GetDbTypeName(dbType)).CreateConnection(); break;
            }
            conn.ConnectionString = connectionString;
            return conn;
        }

        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public static string CreateConnString(int dbIndex = 0)
        {
            DbInfo dbInfo = dbIndex;
            return CreateConnString(dbInfo.DataType, dbInfo.UserID, dbInfo.PassWord, dbInfo.Server, dbInfo.Catalog,
                                    dbInfo.DataVer, dbInfo.ConnectTimeout, dbInfo.PoolMinSize, dbInfo.PoolMaxSize,
                                    dbInfo.Port);
        }

        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="dataType">数据库类型</param>
        /// <param name="userID">账号</param>
        /// <param name="passWord">密码</param>
        /// <param name="server">服务器地址</param>
        /// <param name="catalog">表名</param>
        /// <param name="dataVer">数据库版本</param>
        /// <param name="connectTimeout">链接超时时间</param>
        /// <param name="poolMinSize">连接池最小数量</param>
        /// <param name="poolMaxSize">连接池最大数量</param>
        /// <param name="port">端口</param>
        public static string CreateConnString(DataBaseType dataType, string userID, string passWord, string server,
                                              string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16,
                                              int poolMaxSize = 100, string port = "")
        {
            switch (dataType)
            {
                case DataBaseType.MySql:
                    {
                        return string.Format(
                                "Data Source='{0}';User Id='{1}';Password='{2}';Database='{3}';charset='gbk'", server, userID, passWord, catalog);
                    }
                case DataBaseType.SqlServer:
                    {
                        string connString;
                        if (string.IsNullOrWhiteSpace(userID) && string.IsNullOrWhiteSpace(passWord))
                        {
                            connString = string.Format("Pooling=true;Integrated Security=True;");
                        }
                        else
                        {
                            connString = string.Format("User ID={0};Password={1};Pooling=true;", userID, passWord);
                        }

                        connString += string.Format("Data Source={0};Initial Catalog={1};", server, catalog);

                        if (poolMinSize > 0)
                        {
                            connString += string.Format("Min Pool Size={0};", poolMinSize);
                        }
                        if (poolMaxSize > 0)
                        {
                            connString += string.Format("Max Pool Size={0};", poolMaxSize);
                        }
                        if (connectTimeout > 0)
                        {
                            connString += string.Format("Connect Timeout={0};", connectTimeout);
                        }
                        return connString;
                    }
                case DataBaseType.OleDb:
                    {
                        string connString;
                        switch (dataVer)
                        {
                            case "3.0":
                                {
                                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 3.0;"); break;
                                }
                            case "4.0":
                                {
                                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 4.0;"); break;
                                }
                            case "5.0":
                                {
                                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 5.0;"); break;
                                }
                            case "95":
                                {
                                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 5.0;"); break;
                                }
                            case "97":
                                {
                                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.3.51;"); break;
                                }
                            case "2003":
                                {
                                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;"); break;
                                }
                            default://2007+
                                {
                                    //DR=YES
                                    connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=Excel 12.0;"); break;
                                }
                        }
                        connString += string.Format("Data Source={0};", GetFilePath(server));
                        if (!string.IsNullOrWhiteSpace(userID)) { connString += string.Format("User ID={0};", userID); }
                        if (!string.IsNullOrWhiteSpace(passWord)) { connString += string.Format("Password={0};", passWord); }

                        return connString;
                    }
                case DataBaseType.Xml:
                    {
                        return string.IsNullOrWhiteSpace(server) ? string.Empty : GetFilePath(server);
                    }
                case DataBaseType.SQLite:
                    {
                        var plus = new StringBuilder();
                        plus.AppendFormat("Data Source={0};Min Pool Size={1};Max Pool Size={2};", GetFilePath(server),
                                          poolMinSize, poolMaxSize);
                        if (!string.IsNullOrWhiteSpace(passWord))
                        {
                            plus.AppendFormat("Password={0};", passWord);
                        }
                        if (!string.IsNullOrWhiteSpace(dataVer))
                        {
                            plus.AppendFormat("Version={0};", dataVer);
                        }
                        return plus.ToString();
                    }
                case DataBaseType.Oracle:
                    {
                        if (string.IsNullOrWhiteSpace(port))
                        {
                            port = "1521";
                        }
                        return
                            string.Format(
                                "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={3})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={4})));User Id={1};Password={2};",
                                server, userID, passWord, port, catalog);
                    }
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        ///     压缩数据库
        /// </summary>
        /// <param name="dataType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        public static void Compression(string connetionString, DataBaseType dataType = DataBaseType.SqlServer)
        {
            var db = new DbExecutor(connetionString, dataType, 30);
            switch (dataType)
            {
                case DataBaseType.SQLite:
                    {
                        db.ExecuteNonQuery(CommandType.Text, "VACUUM", null);
                        break;
                    }
                default:
                    throw new NotImplementedException("该数据库不支持该方法！");
            }
        }

        /// <summary>
        ///     压缩数据库
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public static void Compression(int dbIndex)
        {
            DbInfo dbInfo = dbIndex;
            Compression(CreateConnString(dbIndex), dbInfo.DataType);
        }

        /// <summary>
        ///     获取数据库文件的路径
        /// </summary>
        /// <param name="filePath">数据库路径</param>
        private static string GetFilePath(string filePath)
        {
            if (filePath.IndexOf(':') > -1) { return filePath; }

            var fileName = filePath.Replace("/", "\\");
            if (fileName.StartsWith("/")) { fileName = fileName.Substring(1); }

            if (HttpContext.Current != null)
            {
                fileName = HttpContext.Current.Request.PhysicalApplicationPath + "App_Data/" + fileName;
            }
            else
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/" + fileName);
            }
            return fileName;
        }
    }
}
