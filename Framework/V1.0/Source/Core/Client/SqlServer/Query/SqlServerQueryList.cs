using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Assemble;
using FS.Core.Infrastructure;
using FS.Core.Infrastructure.Query;

namespace FS.Core.Client.SqlServer.Query
{
    /// <summary>
    /// 组查询队列（支持批量提交SQL）
    /// </summary>
    public class SqlServerQueryList : IQueryQueueList
    {
        private IQueryQueue _queryQueue;

        public SqlServerQueryList(IQueryQueue queryQueue)
        {
            _queryQueue = queryQueue;
        }

        Expression ExpSelect { get; set; }
        Expression ExpWhere { get; set; }
        Expression ExpOrderBy { get; set; }
        public StringBuilder Sql { get; private set; }

        public void Query()
        {
            var strSelectSql = new SelectAssemble().Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble().Execute(_queryQueue.ExpWhere);
            var strOrderBySql = new OrderByAssemble().Execute(_queryQueue.ExpOrderBy);

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

            _queryQueue.Execute();
        }

        public List<T> Query<T>()
        {
            Query();
            return new Lazy<List<T>>().Value;
        }

        public void Dispose()
        {
            ExpSelect = null;
            ExpWhere = null;
            ExpOrderBy = null;
            Sql.Clear();
            Sql = null;
        }
    }
}
