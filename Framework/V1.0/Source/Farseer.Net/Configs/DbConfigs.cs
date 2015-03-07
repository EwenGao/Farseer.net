using System;
using System.Collections.Generic;
using FS.Core.Data;

namespace FS.Configs
{
    /// <summary>
    ///     全局
    /// </summary>
    public class DbConfigs : BaseConfigs<DbConfig> { }

    /// <summary>
    ///     默认数据库路径
    /// </summary>
    [Serializable]
    public class DbConfig
    {
        /// <summary>
        ///     数据库连接列表，从/App_Data/Db.Configs读取回来
        /// </summary>
        public List<DbInfo> DbList = new List<DbInfo>();
    }

    /// <summary>
    ///     数据库连接配置
    /// </summary>
    public class DbInfo
    {
        /// <summary>
        ///     数据库连接串
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        ///     数据库帐号
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        ///     数据库密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        ///     端口号
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        ///     Oracle SID
        /// </summary>
        public string SID { get; set; }

        /// <summary>
        ///     数据库类型
        /// </summary>
        public DataBaseType DataType { get; set; }

        /// <summary>
        ///     数据库版本
        /// </summary>
        public string DataVer { get; set; }

        /// <summary>
        ///     数据库目录
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        ///     数据库表前缀
        /// </summary>
        public string TablePrefix { get; set; }

        /// <summary>
        ///     最小连接池
        /// </summary>
        public int PoolMinSize { get; set; }

        /// <summary>
        ///     最大连接池
        /// </summary>
        public int PoolMaxSize { get; set; }

        /// <summary>
        ///     数据库连接时间限制，单位秒
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        ///     数据库执行时间限制，单位秒
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        ///     通过索引返回实体
        /// </summary>
        public static implicit operator DbInfo(int index)
        {
            return DbConfigs.ConfigInfo.DbList.Count <= index ? null : DbConfigs.ConfigInfo.DbList[index];
        }
    }
}