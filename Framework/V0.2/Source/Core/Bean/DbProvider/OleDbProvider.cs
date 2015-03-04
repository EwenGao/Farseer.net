using System.Data.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     OleDb数据库提供者
    /// </summary>
    public class OleDbProvider : DbProvider
    {
        /// <summary>
        /// 获取OleDb数据库提供者
        /// </summary>
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OleDb"); }
        }
    }
}