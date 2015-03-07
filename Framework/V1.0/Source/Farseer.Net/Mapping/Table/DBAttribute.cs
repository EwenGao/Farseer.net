using System;
using FS.Core.Data;

namespace FS.Mapping.Table
{
    /// <summary>
    ///     实体类的属性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DBAttribute : Attribute
    {
        /// <summary>
        ///     默认为MSSqlServer 2005、第一个数据库配置
        /// </summary>
        public DBAttribute()
        {
            DbIndex = 0;
            DataType = DataBaseType.SqlServer;
            DataVer = "2005";
        }

        /// <summary>
        ///     表名
        /// </summary>
        public string Name { get; set; }

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
        public DataBaseType DataType { get; set; }

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