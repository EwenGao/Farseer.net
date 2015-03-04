using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using FS.Extend;
using FS.ORM;
using System.Data.SqlClient;

namespace FS.Core.Data
{
    /// <summary>
    ///     数据库操作
    /// </summary>
    public class DbExecutor : IDisposable
    {
        /// <summary>
        ///     数据库执行时间，单位秒
        /// </summary>
        private readonly int CommandTimeout;

        /// <summary>
        ///     连接字符串
        /// </summary>
        private readonly string ConnectionString;

        /// <summary>
        ///     数据类型
        /// </summary>
        private readonly DBType DataType;

        /// <summary>
        ///     数据提供者
        /// </summary>
        public DbProviderFactory Factory;

        /// <summary>
        ///     是否开启事务
        /// </summary>
        public bool IsTransaction;

        /// <summary>
        ///     Sql执行对像
        /// </summary>
        private DbCommand comm;

        /// <summary>
        ///     数据库连接对像
        /// </summary>
        private DbConnection conn;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connnection">数据库连接字符串</param>
        /// <param name="commandTimeout">数据库执行时间，单位秒</param>
        /// <param name="tranLevel">开启事务等级</param>
        public DbExecutor(DBType dbType, string connnection, int commandTimeout, IsolationLevel tranLevel = IsolationLevel.Unspecified)
        {
            ConnectionString = connnection.Replace("|RootDirectory|", AppDomain.CurrentDomain.BaseDirectory, RegexOptions.IgnoreCase);
            CommandTimeout = commandTimeout;
            DataType = dbType;

            OpenTran(tranLevel);
        }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose()
        {
            Close(true);
        }

        /// <summary>
        ///     给定类型，返回DbExecutor
        /// </summary>
        /// <param name="type">实体类型</param>
        public static implicit operator DbExecutor(Type type)
        {
            Mapping map = type;
            return new DbExecutor(map.ClassInfo.DataType, map.ClassInfo.ConnStr, map.ClassInfo.CommandTimeout, IsolationLevel.ReadUncommitted);
        }

        /// <summary>
        ///     开启事务。
        /// </summary>
        /// <param name="tranLevel">事务方式</param>
        public void OpenTran(IsolationLevel tranLevel)
        {
            if (tranLevel != IsolationLevel.Unspecified)
            {
                Open();
                comm.Transaction = conn.BeginTransaction(tranLevel);
                IsTransaction = true;
            }
        }

        /// <summary>
        ///     打开数据库连接
        /// </summary>
        public void Open()
        {
            if (conn == null)
            {
                Factory = DbProviderFactories.GetFactory(DataType.GetName());
                comm = Factory.CreateCommand();
                comm.CommandTimeout = CommandTimeout;

                conn = Factory.CreateConnection();
                conn.ConnectionString = ConnectionString;
                comm.Connection = conn;
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        /// <summary>
        ///     关闭数据库连接
        /// </summary>
        public void Close(bool dispose)
        {
            if ((dispose || comm.Transaction == null) && conn != null && conn.State != ConnectionState.Closed)
            {
                comm.Parameters.Clear();
                comm.Dispose();
                comm = null;
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        /// <summary>
        ///     提交事务
        ///     如果未开启事务则会引发异常
        /// </summary>
        public void Commit()
        {
            if (comm.Transaction == null)
            {
                throw new Exception("未开启事务");
            }
            comm.Transaction.Commit();
        }

        /// <summary>
        ///     添加参数
        /// </summary>
        /// <param name="parameters">SqlParameterCollection的参数对像</param>
        /// <param name="parmsCollection">参数值</param>
        private DbParameterCollection AddParms(DbParameterCollection parmsCollection, DbParameter[] parameters)
        {
            parmsCollection.Clear();
            if (parameters != null)
            {
                foreach (var parms in parameters)
                {
                    parmsCollection.Add(parms);
                }
            }
            return parmsCollection;
        }

        /// <summary>
        ///     返回第一行第一列数据
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (cmdText.IsNullOrEmpty()) { return null; }
            try
            {
                Open();
                comm.CommandType = cmdType;
                comm.CommandText = cmdText;
                AddParms(comm.Parameters, parameters);

                return comm.ExecuteScalar();
            }
            finally
            {
                Close(false);
            }
        }

        /// <summary>
        ///     返回受影响的行数
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (cmdText.IsNullOrEmpty()) { return 0; }
            try
            {
                Open();
                comm.CommandType = cmdType;
                comm.CommandText = cmdText;
                AddParms(comm.Parameters, parameters);

                return comm.ExecuteNonQuery();
            }
            finally
            {
                Close(false);
            }
        }

        /// <summary>
        ///     返回数据(IDataReader)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public IDataReader GetReader(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (cmdText.IsNullOrEmpty()) { return null; }
            try
            {
                Open();
                comm.CommandType = cmdType;
                comm.CommandText = cmdText;
                AddParms(comm.Parameters, parameters);

                return IsTransaction ? comm.ExecuteReader() : comm.ExecuteReader(CommandBehavior.CloseConnection);
            }
            finally
            {
            } // Close();
        }

        /// <summary>
        ///     返回数据(DataSet)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public DataSet GetDataSet(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (cmdText.IsNullOrEmpty()) { return new DataSet(); }
            try
            {
                Open();
                comm.CommandType = cmdType;
                comm.CommandText = cmdText;
                AddParms(comm.Parameters, parameters);
                var ada = Factory.CreateDataAdapter();
                ada.SelectCommand = comm;
                var ds = new DataSet();
                ada.Fill(ds);
                return ds;
            }
            finally
            {
                Close(false);
            }
        }

        /// <summary>
        ///     返回数据(DataTable)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            var ds = GetDataSet(cmdType, cmdText, parameters);
            return ds.Tables.Count == 0 ? new DataTable() : ds.Tables[0];
        }

        /// <summary>
        /// 指量操作数据（仅支付Sql Server)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据</param>
        public void ExecuteSqlBulkCopy(string tableName, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) { return; }

            try
            {
                Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)conn, SqlBulkCopyOptions.Default, (SqlTransaction)comm.Transaction))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.BulkCopyTimeout = 3600;
                    bulkCopy.WriteToServer(dt);

                    if (bulkCopy != null) { bulkCopy.Close(); }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally
            {
                Close(false);
            }
        }
    }

    /// <summary>
    ///     数据库类型
    /// </summary>
    public enum DBType
    {
        /// <summary>
        ///     SqlServer数据库
        /// </summary>
        [Display(Name = "System.Data.SqlClient")]
        SqlServer,

        /// <summary>
        ///     Access数据库
        /// </summary>
        [Display(Name = "System.Data.OleDb")]
        OleDb,

        /// <summary>
        ///     MySql数据库
        /// </summary>
        [Display(Name = "MySql.Data.MySqlClient")]
        MySql,

        /// <summary>
        ///     Xml
        /// </summary>
        [Display(Name = "System.Linq.Xml")]
        Xml,

        /// <summary>
        ///     SQLite
        /// </summary>
        [Display(Name = "System.Data.SQLite")]
        SQLite,

        /// <summary>
        ///     Oracle
        /// </summary>
        [Display(Name = "System.Data.OracleClient")]
        Oracle,
    }

    /// <summary>
    ///     字段类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        ///     整型
        /// </summary>
        [Display(Name = "Int")]
        Int,

        /// <summary>
        ///     布尔型
        /// </summary>
        [Display(Name = "Bit")]
        Bit,

        /// <summary>
        ///     可变字符串
        /// </summary>
        [Display(Name = "Varchar")]
        Varchar,

        /// <summary>
        ///     可变字符串（双字节）
        /// </summary>
        [Display(Name = "Nvarchar")]
        Nvarchar,

        /// <summary>
        ///     不可变字符串
        /// </summary>
        [Display(Name = "Char")]
        Char,

        /// <summary>
        ///     不可变字符串（双字节）
        /// </summary>
        [Display(Name = "NChar")]
        NChar,

        /// <summary>
        ///     不可变文本
        /// </summary>
        [Display(Name = "Text")]
        Text,

        /// <summary>
        ///     不可变文本
        /// </summary>
        [Display(Name = "Ntext")]
        Ntext,

        /// <summary>
        ///     日期
        /// </summary>
        [Display(Name = "DateTime")]
        DateTime,

        /// <summary>
        ///     短整型
        /// </summary>
        [Display(Name = "Smallint")]
        Smallint,

        /// <summary>
        ///     浮点
        /// </summary>
        [Display(Name = "Float")]
        Float,
    }
}