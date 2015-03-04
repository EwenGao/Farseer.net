using System.Text;
using FS.Core.Model;

namespace FS.Core.Bean
{
    /// <summary>
    ///     Oracle数据库Sql生成
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    internal class OracleBuilder<TInfo> : DbBuilder<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     Oracle数据库Sql生成
        /// </summary>
        /// <param name="tableName">表名称</param>
        internal OracleBuilder(string tableName = "")
            : base(tableName)
        {
            TableName = dbProvider.CreateTableAegis(TableName);
        }

        public override string ToInfo()
        {
            if (WhereString.Length == 0) { WhereString.Append("WHERE "); }
            else { WhereString.Append(" AND "); }

            return string.Format("select {0} from {1} {2} rownum <=1 {3};", GetFields(), TableName, WhereString, SortString);
        }

        public override string GetValue()
        {
            if (WhereString.Length == 0)   { WhereString.Append("WHERE ");    }
            else { WhereString.Append(" AND ");    }

            return string.Format("SELECT {0} FROM {1} {2} rownum <=1 {3};", GetFields(), TableName, WhereString,
                                 SortString);
        }

        public override string ToTable(int pageSize, int pageIndex)
        {
            if (pageIndex == 1)    { return ToTable(pageSize);    }

            return string.Format(
                    "SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT {0} FROM {3} {4} {5}) A WHERE ROWNUM <= {2} ) WHERE RN > {1};",
                    GetFields(), pageSize * (pageIndex - 1), pageSize * pageIndex, TableName, WhereString, SortString);
        }

        public override string ToTable(int top = 0)
        {
            var strFields = SelectString.Length == 0 ? "*" : SelectString.ToString();

            var topString = new StringBuilder();
            if (top > 0)
            {
                if (WhereString.Length > 0)   {  topString.Append(" AND ");  }
                topString.Append(string.Format("ROWNUM <= {0}", top));
            }
            return string.Format("SELECT {0} FROM {1} {2} {3} {4};", GetFields(), TableName, WhereString, topString,
                                 SortString);
        }

        public override string ToTableByRand(int top = 0)
        {
            var topString = top > 0 ? string.Format("WHERE ROWNUM <= {0}", top) : string.Empty;
            return string.Format("SELECT * FROM (SELECT {0} FROM {1} {2} ORDER BY dbms_random.value) {3};", GetFields(),
                                 TableName, WhereString, topString);
        }
    }
}