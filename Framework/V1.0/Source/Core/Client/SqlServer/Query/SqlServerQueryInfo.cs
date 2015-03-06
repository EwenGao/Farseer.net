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
    public class SqlServerQueryInfo : IQueryQueueInfo
    {
        Expression ExpSelect { get; set; }
        Expression ExpWhere { get; set; }
        Expression ExpOrderBy { get; set; }
        
        private IQueryQueue _queryQueue;

        public SqlServerQueryInfo(IQueryQueue queryQueue)
        {
            _queryQueue = queryQueue;
        }

        public void Query( )
        {
            query.GroupQueryQueue.Sql = new StringBuilder();

            var strSelectSql = new SelectAssemble().Execute(query.ExpSelect);
            var strWhereSql = new WhereAssemble().Execute(query.ExpWhere);
            var strOrderBySql = new OrderByAssemble().Execute(query.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            query.GroupQueryQueue.Sql.Append(string.Format("select top 1 {0} ", strSelectSql));

            query.GroupQueryQueue.Sql.Append(string.Format("from {0} ", query.TableName));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                query.GroupQueryQueue.Sql.Append(string.Format("where {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                query.GroupQueryQueue.Sql.Append(string.Format("orderby {0} ", strOrderBySql));
            }

            query.Init();
        }

        public T Query<T>( ) where T : class
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
