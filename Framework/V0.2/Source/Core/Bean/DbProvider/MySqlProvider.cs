using System.Data.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     MySql数据库提供者
    /// </summary>
    public class MySqlProvider : DbProvider
    {
        /// <summary>
        /// 获取MySql数据库提供者
        /// </summary>
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("MySql.Data.MySqlClient"); }
        }

        /// <summary>
        /// 获取MySql保护符
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public override string CreateTableAegis(string fieldName)
        {
            // 如果不是字段名，则直接返回
            if (!IsField(fieldName)) { return fieldName; }
            return string.Format("`{0}`", fieldName);
        }
    }
}