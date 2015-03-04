using System;
using System.Data;
using System.IO;
using System.Web;
using FS.Configs;
using FS.Core.Data;
using FS.Core.Model;
using FS.Extend;
using FS.Mapping.Table;
using FS.Utils.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库工厂模式
    /// </summary>
    public class DbFactory
    {
        /// <summary>
        ///     创建数据库Sql生成
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="tableName">表名称</param>
        internal static DbBuilder<TInfo> CreateDbBuilder<TInfo>(string tableName = "") where TInfo : ModelInfo, new()
        {
            TableMap map = typeof(TInfo);
            if (tableName.IsNullOrEmpty()) { tableName = map.ClassInfo.Name; }
            switch (map.ClassInfo.DataType)
            {
                case DataBaseType.OleDb: return new OleDbBuilder<TInfo>(tableName);
                case DataBaseType.MySql: return new MySqlBuilder<TInfo>(tableName);
                //case DataBaseType.Xml: return new XmlBean<TInfo>(connetionString);
                case DataBaseType.SQLite: return new SQLiteBuilder<TInfo>(tableName);
                case DataBaseType.Oracle: return new OracleBuilder<TInfo>(tableName);
                default: return new SqlServerBuilder<TInfo>(tableName);
            }
        }

        /// <summary>
        ///     创建数据库表达式树解析器
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        internal static DbVisit<TInfo> CreateDbVisit<TInfo>() where TInfo : ModelInfo, new()
        {
            TableMap map = typeof(TInfo);
            switch (map.ClassInfo.DataType)
            {
                case DataBaseType.OleDb: return new OleDbVisit<TInfo>();
                case DataBaseType.MySql: return new MySqlVisit<TInfo>();
                //case DataBaseType.Xml: return new XmlBean<TInfo>(connetionString);
                case DataBaseType.SQLite: return new SQLiteVisit<TInfo>();
                case DataBaseType.Oracle: return new OracleVisit<TInfo>();
                default: return new SqlServerVisit<TInfo>();
            }
        }

        /// <summary>
        ///     创建修改数据库类
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="tableName">要操作的表名</param>
        public static DbOperate CreateDbOperate(DataBaseType dbType, string connetionString, string tableName)
        {
            switch (dbType)
            {
                case DataBaseType.OleDb:
                    return new OleDbOperate(connetionString, tableName);
                case DataBaseType.MySql:
                    return new MySqlOperate(connetionString, tableName);
                case DataBaseType.SQLite:
                    return new SQLiteOperate(connetionString, tableName);
                case DataBaseType.Oracle:
                    return new OracleOperate(connetionString, tableName);
                case DataBaseType.SqlServer:
                    return new SqlServerOperate(connetionString, tableName);
                default:
                    {
                        throw new Exception("该数据库不支持此操作！");
                    }
            }
        }

        /// <summary>
        ///     创建数据库提供者
        /// </summary>
        public static DbProvider CreateDbProvider<TInfo>() where TInfo : ModelInfo, new()
        {
            TableMap Map = typeof(TInfo);
            switch (Map.ClassInfo.DataType)
            {
                case DataBaseType.OleDb:
                    return new OleDbProvider();
                case DataBaseType.MySql:
                    return new MySqlProvider();
                case DataBaseType.SQLite:
                    return new SQLiteProvider();
                case DataBaseType.Oracle:
                    return new OracleProvider();
                case DataBaseType.SqlServer:
                    return new SqlServerProvider();
                default: { throw new Exception("该数据库不支持此操作！"); }
            }
        }

        /// <summary>
        ///     创建数据库提供者
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        public static DbProvider CreateDbProvider(DataBaseType dbType)
        {
            switch (dbType)
            {
                case DataBaseType.OleDb:
                    return new OleDbProvider();
                case DataBaseType.MySql:
                    return new MySqlProvider();
                case DataBaseType.SQLite:
                    return new SQLiteProvider();
                case DataBaseType.Oracle:
                    return new OracleProvider();
                case DataBaseType.SqlServer:
                    return new SqlServerProvider();
                default: { throw new Exception("该数据库不支持此操作！"); }
            }
        }

        /// <summary>
        ///     创建数据库操作
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="tranLevel">开启事务等级</param>
        public static DbExecutor CreateDbExecutor<TInfo>(IsolationLevel tranLevel = IsolationLevel.Serializable) where TInfo : ModelInfo, new()
        {
            TableMap map = typeof(TInfo);
            var dataType = map.ClassInfo.DataType;
            var connetionString = map.ClassInfo.ConnStr;
            var commandTimeout = map.ClassInfo.CommandTimeout;

            return new DbExecutor(dataType, connetionString, commandTimeout, tranLevel);
        }

        /// <summary>
        ///     创建数据库操作
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        /// <param name="tranLevel">开启事务等级</param>
        public static DbExecutor CreateDbExecutor(int dbIndex = 0, IsolationLevel tranLevel = IsolationLevel.Unspecified)
        {
            DbInfo dbInfo = dbIndex;
            return new DbExecutor(dbInfo.DataType, CreateConnString(dbIndex), dbInfo.CommandTimeout, tranLevel);
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
                        if (userID.IsNullOrEmpty() && passWord.IsNullOrEmpty())
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
                        if (!userID.IsNullOrEmpty()) { connString += string.Format("User ID={0};", userID); }
                        if (!passWord.IsNullOrEmpty()) { connString += string.Format("Password={0};", passWord); }

                        return connString;
                    }
                case DataBaseType.Xml:
                    {
                        return server.IsNullOrEmpty() ? string.Empty : GetFilePath(server);
                    }
                case DataBaseType.SQLite:
                    {
                        var plus = new StrPlus();
                        plus.AppendFormat("Data Source={0};Min Pool Size={1};Max Pool Size={2};", GetFilePath(server),
                                          poolMinSize, poolMaxSize);
                        if (!passWord.IsNullOrEmpty())
                        {
                            plus.AppendFormat("Password={0};", passWord);
                        }
                        if (!dataVer.IsNullOrEmpty())
                        {
                            plus.AppendFormat("Version={0};", dataVer);
                        }
                        return plus.Value;
                    }
                case DataBaseType.Oracle:
                    {
                        if (port.IsNullOrEmpty())
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
            var db = new DbExecutor(dataType, connetionString, 30);
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

            var fileName = Files.ConvertPath(filePath);
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