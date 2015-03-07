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
    public class SqlServerQueryDelete : IQueryQueueDelete
    {
        Expression ExpSelect { get; set; }
        Expression ExpWhere { get; set; }
        Expression ExpOrderBy { get; set; }

        private IQueryQueue _queryQueue;

        public SqlServerQueryDelete(IQueryQueue queryQueue)
        {
            _queryQueue = queryQueue;
        }

        public void Query()
        {
            _queryQueue.Sql = new StringBuilder();

            var strSelectSql = new SelectAssemble().Execute(_queryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble().Execute(_queryQueue.ExpWhere);
            var strOrderBySql = new OrderByAssemble().Execute(_queryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryQueue.Sql.Append(string.Format("select top 1 {0} ", strSelectSql));

            _queryQueue.Sql.Append(string.Format("from {0} ", _queryQueue.TableName));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryQueue.Sql.Append(string.Format("where {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryQueue.Sql.Append(string.Format("orderby {0} ", strOrderBySql));
            }

            _queryQueue.Init();
        }

        public T Query<T>() where T : class
        {
            Query(query);
            return query.Database.GetReader(System.Data.CommandType.Text, Sql.ToString()).ToInfo<T>();
        }

        public void Dispose()
        {
            ExpSelect = null;
            ExpWhere = null;
            ExpOrderBy = null;
        }
    }
}
