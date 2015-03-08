using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Assemble;
using FS.Core.Infrastructure;
using FS.Core.Infrastructure.Query;
using FS.Extend;

namespace FS.Core.Client.SqlServer.Query
{
    /// <summary>
    /// 组查询队列（支持批量提交SQL）
    /// </summary>
    public class SqlServerQueryList : IQueryQueueList
    {
        private readonly IQuery _queryProvider;

        public SqlServerQueryList(IQuery queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public void Query()
        {
            var strSelectSql = new SelectAssemble().Execute(_queryProvider.QueryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble().Execute(_queryProvider.QueryQueue.ExpWhere);
            var strOrderBySql = new OrderByAssemble().Execute(_queryProvider.QueryQueue.ExpOrderBy);

            _queryProvider.QueryQueue.Sql = new StringBuilder();

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryProvider.QueryQueue.Sql.Append(string.Format("select {0} ", strSelectSql));

            _queryProvider.QueryQueue.Sql.Append(string.Format("from {0} ", _queryProvider.TableContext.TableName));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("where {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("orderby {0} ", strOrderBySql));
            }
        }

        public List<T> Query<T>() where T : class, new()
        {
            Query();
            List<T> t;
            using (var reader = _queryProvider.TableContext.Database.GetReader(System.Data.CommandType.Text, _queryProvider.QueryQueue.Sql.ToString()))
            {
                t = reader.ToList<T>();
                reader.Close();
            }
            _queryProvider.Init();
            return t;
        }
    }
}
