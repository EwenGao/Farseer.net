using System;
using System.Data;
using System.Text;
using FS.Core.Data;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库修改表结构
    /// </summary>
    public class MySqlOperate : DbOperate
    {
        /// <summary>
        ///     MySql数据库操作
        /// </summary>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="tableName">要操作的表名</param>
        public MySqlOperate(string connetionString, string tableName) : base(connetionString, tableName)
        {
            dbProvider = new MySqlProvider();
            dbExecutor = new DbExecutor(DataBaseType.MySql, connetionString, 60);
        }

        /// <summary>
        ///     判断表是否存在
        /// </summary>
        public override bool IsExistTable()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT count(*) from information_schema.TABLES WHERE table_name ='{0}'", TableName);

            return dbExecutor.ExecuteScalar(CommandType.Text, sql.ToString(), null).ConvertType(0) > 0;
        }

        /// <summary>
        ///     获取字段创建字符串
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="isPrimaryKey">是否为主键</param>
        /// <param name="isIdentity">是否自增长</param>
        /// <param name="isNotNull">是否不为空</param>
        /// <param name="fieldType">字段类型</param>
        /// <param name="fieldLength">字段长度</param>
        /// <param name="fieldDefaultValue">字段默认值</param>
        protected override string CreateFieldString(string fieldName, bool isPrimaryKey, bool isIdentity, bool isNotNull,
                                                    FieldType fieldType, int? fieldLength, object fieldDefaultValue)
        {
            if (fieldDefaultValue is Enum)
            {
                fieldDefaultValue = (int) fieldDefaultValue;
            }
            if (fieldDefaultValue != null)
            {
                if (
                    !fieldDefaultValue.ToString().StartsWith("'") &&
                    (fieldType == FieldType.Char ||
                     fieldType == FieldType.NChar ||
                     fieldType == FieldType.Ntext ||
                     fieldType == FieldType.Nvarchar ||
                     fieldType == FieldType.Text ||
                     fieldType == FieldType.Varchar))
                {
                    fieldDefaultValue = "'" + fieldDefaultValue + "'";
                }
            }
            var sql = new StringBuilder();

            sql.AppendFormat("{0} ", dbProvider.CreateTableAegis(fieldName));
            sql.AppendFormat("{0} ", fieldType.ToString());
            if (fieldLength != null)
            {
                sql.AppendFormat("({0}) ", fieldLength);
            }
            if (isIdentity)
            {
                sql.Append("AUTO_INCREMENT ");
            }
            sql.Append(isNotNull ? "NOT NULL " : "NULL ");
            if (isPrimaryKey)
            {
                sql.Append("PRIMARY KEY ");
            }
            if (fieldDefaultValue != null && fieldDefaultValue.ToString().Length > 0)
            {
                sql.AppendFormat("DEFAULT {0} ", fieldDefaultValue);
            }
            ;

            return sql.ToString();
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