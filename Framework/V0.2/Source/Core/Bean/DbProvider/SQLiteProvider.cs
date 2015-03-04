using System.Data.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     SqLite数据库提供者
    /// </summary>
    public class SQLiteProvider : DbProvider
    {
        /// <summary>
        /// 获取SqLite数据库提供者
        /// </summary>
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SQLite"); }
        }
    }
}