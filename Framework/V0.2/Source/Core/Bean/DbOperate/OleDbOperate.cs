using System.Data;
using System.Text;
using FS.Core.Data;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库修改表结构
    /// </summary>
    public class OleDbOperate : DbOperate
    {
        /// <summary>
        ///     OleDb数据库操作
        /// </summary>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="tableName">要操作的表名</param>
        public OleDbOperate(string connetionString, string tableName) : base(connetionString, tableName)
        {
            dbProvider = new OleDbProvider();
            dbExecutor = new DbExecutor(DataBaseType.OleDb, connetionString, 60);
        }

        /// <summary>
        ///     判断表是否存在
        /// </summary>
        public override bool IsExistTable()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("Select * from MSysObjects where name='{0}' and type=1", TableName);

            return dbExecutor.ExecuteScalar(CommandType.Text, sql.ToString(), null).ConvertType(0) > 0;
        }

        /// <summary>
        ///     修改表名称
        /// </summary>
        /// <param name="tableName">新表名称</param>
        public override bool ReNameTable(string tableName)
        {
            if (!IsExistTable())
            {
                return false;
            }
            dbExecutor.ExecuteNonQuery(CommandType.Text,
                                       string.Format("select * into {0} from {1}", TableName, TableName), null);
            DropTable();
            return true;
        }

        /// <summary>
        ///     判断字段是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public override bool IsExistField(string fieldName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(
                "select count(0) from information_schema.columns where  table_name ='{0}' and column_name='{1}'",
                TableName, fieldName);

            return dbExecutor.ExecuteScalar(CommandType.Text, sql.ToString(), null).ConvertType(0) > 0;
        }
    }
}