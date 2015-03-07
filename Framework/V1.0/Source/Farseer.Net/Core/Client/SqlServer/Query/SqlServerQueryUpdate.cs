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
    public class SqlServerQueryUpdate : IQueryQueueUpdate
    {
        private readonly IQuery _queryProvider;

        public SqlServerQueryUpdate(IQuery queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public void Query()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();

            var strSelectSql = new SelectAssemble().Execute(_queryProvider.QueryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble().Execute(_queryProvider.QueryQueue.ExpWhere);
            var strOrderBySql = new OrderByAssemble().Execute(_queryProvider.QueryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryProvider.QueryQueue.Sql.Append(string.Format("select top 1 {0} ", strSelectSql));

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

        public int Query<T>(T entity) where T : class
        {
            Query();
            var result = _queryProvider.TableContext.Database.ExecuteNonQuery(System.Data.CommandType.Text, _queryProvider.QueryQueue.Sql.ToString());
            _queryProvider.Execute();
            return result;
        }
    }
}
