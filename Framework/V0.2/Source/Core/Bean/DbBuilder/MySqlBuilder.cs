using FS.Core.Model;

namespace FS.Core.Bean
{
    /// <summary>
    ///     MySql数据库持久化
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    internal class MySqlBuilder<TInfo> : DbBuilder<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     MySql数据库Sql生成
        /// </summary>
        /// <param name="tableName">表名称</param>
        internal MySqlBuilder(string tableName = "") : base(tableName)
        {
            TableName = dbProvider.CreateTableAegis(TableName);
        }

        public override string ResetIdentity()
        {
            return string.Format("ALTER TABLE `{0}` AUTO_INCREMENT=1;", TableName);
        }

        public override string ToInfo()
        {
            return string.Format("select {0} from {1} {2} {3} LIMIT 1;", GetFields(), TableName, WhereString, SortString);
        }

        public override string ToTable(int top = 0)
        {
            var topString = top > 0 ? string.Format("LIMIT {0}", top) : string.Empty;
            return string.Format("SELECT {0} FROM {1} {2} {3} {4};", GetFields(), TableName, WhereString, SortString,
                                 topString);
        }

        public override string ToTable(int pageSize, int pageIndex)
        {
            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4},{5};", GetFields(), TableName, WhereString,
                                 SortString, pageSize*(pageIndex - 1), pageSize);
        }

        public override string ToTableByRand(int top = 0)
        {
            var topString = top > 0 ? string.Format("Limit {0}", top) : string.Empty;
            return string.Format("SELECT {0} FROM {1} {2} order By Rand() {3};", GetFields(), TableName, WhereString,
                                 topString);
        }
    }
}