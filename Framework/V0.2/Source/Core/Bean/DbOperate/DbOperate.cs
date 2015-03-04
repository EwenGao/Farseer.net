using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FS.Core.Data;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库修改
    /// </summary>
    public abstract class DbOperate
    {
        /// <summary>
        ///     数据库操作
        /// </summary>
        protected DbExecutor dbExecutor;

        /// <summary>
        ///     数据库提供者
        /// </summary>
        protected DbProvider dbProvider;

        /// <summary>
        ///     初始化类
        /// </summary>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="tableName">要操作的表名</param>
        protected DbOperate(string connetionString, string tableName)
        {
            ConnetionString = connetionString;
            TableName = tableName;
        }

        /// <summary>
        ///     连接字符串
        /// </summary>
        protected string ConnetionString { get; set; }

        /// <summary>
        ///     要操作的表名
        /// </summary>
        protected string TableName { get; set; }

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
        protected virtual string CreateFieldString(string fieldName, bool isPrimaryKey, bool isIdentity, bool isNotNull, FieldType fieldType, int? fieldLength, object fieldDefaultValue)
        {
            if (fieldDefaultValue is Enum)
            {
                fieldDefaultValue = (int)fieldDefaultValue;
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
                sql.Append("IDENTITY(1,1) ");
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
        ///     创建表
        /// </summary>
        /// <param name="lstCreateField">创建的字段列表</param>
        public virtual bool CreateTable(List<string> lstCreateField)
        {
            var sql = new StringBuilder();

            sql.AppendFormat("CREATE TABLE {0} (", dbProvider.CreateTableAegis(TableName));
            sql.Append(lstCreateField.ToString(","));
            sql.Append(");");


            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return IsExistTable();
        }

        /// <summary>
        ///     修改表名称
        /// </summary>
        /// <param name="tableName">新表名称</param>
        public virtual bool ReNameTable(string tableName)
        {
            if (!IsExistTable())
            {
                return false;
            }
            var sql = new StringBuilder();
            sql.AppendFormat("ALTER TABLE {0} RENAME TO {1}", TableName, tableName);

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     删除表
        /// </summary>
        public virtual bool DropTable()
        {
            if (!IsExistTable())
            {
                return true;
            }

            var sql = new StringBuilder();
            sql.AppendFormat("DROP TABLE {0}", TableName);

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     判断表是否存在
        /// </summary>
        public virtual bool IsExistTable()
        {
            return false;
        }

        /// <summary>
        ///     添加字段
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="isPrimaryKey">是否为主键</param>
        /// <param name="isIdentity">是否自增长</param>
        /// <param name="isNotNull">是否不为空</param>
        /// <param name="fieldType">字段类型</param>
        /// <param name="fieldLength">字段长度</param>
        /// <param name="fieldDefaultValue">字段默认值</param>
        public virtual bool AddField(string fieldName, bool isPrimaryKey, bool isIdentity, bool isNotNull, FieldType fieldType, int? fieldLength, object fieldDefaultValue)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("alter table {0} ADD ", TableName);
            sql.Append(CreateFieldString(fieldName, isPrimaryKey, isIdentity, isNotNull, fieldType, fieldLength,
                                         fieldDefaultValue));

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     修改字段
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="isPrimaryKey">是否为主键</param>
        /// <param name="isIdentity">是否自增长</param>
        /// <param name="isNotNull">是否不为空</param>
        /// <param name="fieldType">字段类型</param>
        /// <param name="fieldLength">字段长度</param>
        /// <param name="fieldDefaultValue">字段默认值</param>
        public virtual bool UpdateField(string fieldName, bool isPrimaryKey, bool isIdentity, bool isNotNull, FieldType fieldType, int? fieldLength, object fieldDefaultValue)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("alter table {0} alter column ", TableName);
            sql.Append(CreateFieldString(fieldName, isPrimaryKey, isIdentity, isNotNull, fieldType, fieldLength,
                                         fieldDefaultValue));

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     修改字段名称
        /// </summary>
        /// <param name="oldFieldName">旧字段名称</param>
        /// <param name="newFieldName">新字段名称</param>
        public virtual bool RenameField(string oldFieldName, string newFieldName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("alter table {0} rename column {1} to {2}", TableName, oldFieldName, newFieldName);

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     删除字段
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public virtual bool DropField(string fieldName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("alter table {0} drop column {1}", TableName, fieldName);

            dbExecutor.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
            return true;
        }

        /// <summary>
        ///     判断字段是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public virtual bool IsExistField(string fieldName)
        {
            var dt = dbExecutor.GetDataTable(CommandType.Text,
                                                   string.Format("select * from {0} where 1<>1", TableName), null);
            return dt.Columns.Contains(fieldName);
        }
    }
}