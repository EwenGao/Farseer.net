using System.Data;
using System.Text;
using FS.Core.Data;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库修改表结构
    /// </summary>
    public class SqlServerOperate : DbOperate
    {
        /// <summary>
        ///     SqlServer数据库操作
        /// </summary>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="tableName">要操作的表名</param>
        public SqlServerOperate(string connetionString, string tableName) : base(connetionString, tableName)
        {
            dbProvider = new SqlServerProvider();
            dbExecutor = new DbExecutor(DataBaseType.SqlServer, connetionString, 60);
        }

        /// <summary>
        ///     判断表是否存在
        /// </summary>
        public override bool IsExistTable()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("select count(*) from sysobjects where name='{0}' and xtype= 'u'", TableName);

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
            var sql = new StringBuilder();
            sql.AppendFormat("EXEC sp_rename '{0}', '{1}'", TableName, tableName);

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     修改字段名称
        /// </summary>
        /// <param name="oldFieldName">旧字段名称</param>
        /// <param name="newFieldName">新字段名称</param>
        /// <returns></returns>
        public override bool RenameField(string oldFieldName, string newFieldName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("EXEC sp_rename '{0}.[{1}]', '{2}', 'COLUMN'", TableName, oldFieldName, newFieldName);

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
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
                "select count(0) from sysobjects a inner join syscolumns b on a.id=b.id where a.id= object_id('{0}') and b.name='{1}'",
                TableName, fieldName);

            return dbExecutor.ExecuteScalar(CommandType.Text, sql.ToString(), null).ConvertType(0) > 0;
        }
    }
}