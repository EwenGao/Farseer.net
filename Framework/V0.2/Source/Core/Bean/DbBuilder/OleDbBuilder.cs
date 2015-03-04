using FS.Core.Model;

namespace FS.Core.Bean
{
    /// <summary>
    ///     OleDb数据库Sql生成
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    internal class OleDbBuilder<TInfo> : DbBuilder<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     OleDb数据库Sql生成
        /// </summary>
        /// <param name="tableName">表名称</param>
        internal OleDbBuilder(string tableName = "") : base(tableName)
        {
            TableName = dbProvider.CreateTableAegis(TableName);
        }

        public override string ToTableByRand(int top = 0)
        {
            var topString = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            return string.Format("SELECT {0} {1} FROM {2} ORDER {3} BY Rnd(-(TestID+\" & Rnd() & \"));", topString,
                                 GetFields(), TableName, WhereString);
        }
    }
}