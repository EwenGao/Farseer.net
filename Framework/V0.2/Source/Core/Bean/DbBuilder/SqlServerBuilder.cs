using System.Text;
using FS.Core.Model;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     SqlServer数据库Sql生成
    /// </summary>
    /// <typeparam name="TInfo">实体类</typeparam>
    internal class SqlServerBuilder<TInfo> : DbBuilder<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     SqlServer数据库Sql生成
        /// </summary>
        /// <param name="tableName">表名称</param>
        internal SqlServerBuilder(string tableName = "")
            : base(tableName)
        {
            TableName = dbProvider.CreateTableAegis(TableName);
        }

        public override string Insert(TInfo info)
        {
            ParamsList = GetParameter(info);
            if (ParamsList.Count == 0)
            {
                return string.Empty;
            }

            var indexHaveValue = Map.GetModelInfo().Key != null && Map.GetModelInfo().Key.GetValue(info, null) != null;

            //要插入的表
            var sql = new StringBuilder();
            if (!Map.IndexName.IsNullOrEmpty() && indexHaveValue)
            {
                sql.AppendFormat("SET IDENTITY_INSERT {0} ON ; ", TableName);
            }

            sql.AppendFormat("INSERT INTO {0} ", TableName);

            #region 要插入的值

            var strFields = new StringBuilder("(");
            var strValues = new StringBuilder("(");

            foreach (var param in ParamsList)
            {
                strFields.AppendFormat("{0},", dbProvider.CreateTableAegis(param.ParameterName.Substring(1)));
                strValues.AppendFormat("{0},", param.ParameterName);
            }

            sql.AppendFormat(strFields.ToString().DelEndOf(",") + ") VALUES " + strValues.ToString().DelEndOf(",") + ")");

            #endregion

            if (!Map.IndexName.IsNullOrEmpty() && indexHaveValue)
            {
                sql.AppendFormat("; SET IDENTITY_INSERT {0} OFF ", TableName);
            }

            return sql.ToString() + ";";
        }

        public override string LastIdentity()
        {
            return "SELECT @@IDENTITY;";
        }

        public override string Update(TInfo info)
        {
            var param = GetParameter(info);

            #region 如果主键有值，则取消修改主键的SQL

            var indexHaveValue = Map.GetModelInfo().Key != null && Map.GetModelInfo().Key.GetValue(info, null) != null;
            if (indexHaveValue)
            {
                param.RemoveAll(o => o.ParameterName.IsEquals(dbProvider.ParamsPrefix + Map.IndexName));
            }

            #endregion

            if (param.Count == 0) return string.Empty;

            // 要更新的表
            var sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0} SET ", TableName);

            // 要更新的字段
            foreach (var parm in param)
            {
                sql.AppendFormat("{0} = {1} ,", dbProvider.CreateTableAegis(parm.ParameterName.Substring(1)),
                                 parm.ParameterName);
            }

            ParamsList.AddRange(param);

            return sql.ToString().DelEndOf(",") + WhereString + ";";
        }

        public override string ToTable(int pageSize, int pageIndex)
        {
            if (Map.ClassInfo.DataVer == "2000") { return base.ToTable(pageSize, pageIndex); }
            if (pageIndex == 1) { return base.ToTable(pageSize); }

            if (SortString.Length == 0) { SortString.AppendFormat("ORDER BY {0} ASC", Map.IndexName); }
            return string.Format(
                    "SELECT {0} FROM (SELECT {0},ROW_NUMBER() OVER({1}) as Row FROM {2} {3}) a WHERE Row BETWEEN {4} AND {5};",
                    GetFields(), SortString, TableName, WhereString, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }

        public override string ResetIdentity()
        {
            return string.Format("dbcc checkident({0},reseed,0);", TableName);
        }
    }
}