using FS.Core.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.ORM
{
    /// <summary>
    ///     实体类的属性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DBAttribute : TableAttribute
    {
        /// <summary>
        ///     默认为MSSqlServer 2005、第一个数据库配置
        /// </summary>
        public DBAttribute(string name) : base(name)
        {
            DbIndex = 0;
            DataType = DBType.SqlServer;
            DataVer = "2008";
        }

        /// <summary>
        ///     设置数据库连接配置(Dbconfig)的索引项
        /// </summary>
        public int DbIndex { get; set; }

        /// <summary>
        ///     设置数据库连接字符串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        ///     设置数据库类型
        /// </summary>
        public DBType DataType { get; set; }

        /// <summary>
        ///     设置数据库版本
        /// </summary>
        public string DataVer { get; set; }

        /// <summary>
        ///     设置数据库执行T-SQL时间，单位秒默认是30秒
        /// </summary>
        public int CommandTimeout { get; set; }
    }
}