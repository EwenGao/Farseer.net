using FS.Core.Assemble;
using FS.Core.Client.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 组查询队列（支持批量提交SQL）
    /// </summary>
    public class SqlServerQueryQueueList : IQueryQueueExecute
    {
        Expression ExpSelect { get; set; }
        Expression ExpWhere { get; set; }
        Expression ExpOrderBy { get; set; }
        private StringBuilder Sql { get; set; }

        public void Commit(IQuery query)
        {

        }

        public void Query(IQuery query)
        {
            var strSelectSql = new SelectAssemble().Execute(query.ExpSelect);
            var strWhereSql = new WhereAssemble().Execute(query.ExpWhere);
            var strOrderBySql = new OrderByAssemble().Execute(query.ExpOrderBy);

            Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            Sql.Append(string.Format("select {0} ", strSelectSql));

            Sql.Append(string.Format("from {0} ", query.TableName));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                Sql.Append(string.Format("where {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Sql.Append(string.Format("orderby {0} ", strOrderBySql));
            }

            query.Execute();
        }
    }
}
