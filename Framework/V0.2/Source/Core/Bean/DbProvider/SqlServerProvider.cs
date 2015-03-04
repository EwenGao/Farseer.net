using System.Data.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     SqlServer数据库提供者
    /// </summary>
    public class SqlServerProvider : DbProvider
    {
        /// <summary>
        /// 获取SqlServer数据库提供者
        /// </summary>
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.SqlClient"); }
        }
    }
}