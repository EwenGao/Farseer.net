using System.Data;
using System.Text;
using FS.Core.Data;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库修改表结构
    /// </summary>
    public class OracleOperate : DbOperate
    {
        /// <summary>
        ///     Oracle数据库操作
        /// </summary>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="tableName">要操作的表名</param>
        public OracleOperate(string connetionString, string tableName) : base(connetionString, tableName)
        {
            dbProvider = new OracleProvider();
            dbExecutor = new DbExecutor(DataBaseType.Oracle, connetionString, 60);
        }

        /// <summary>
        ///     判断表是否存在
        /// </summary>
        public override bool IsExistTable()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("select count(*) into a from user_tables where table_name= '{0} '", TableName);

            return dbExecutor.ExecuteScalar(CommandType.Text, sql.ToString(), null).ConvertType(0) > 0;
        }
    }
}