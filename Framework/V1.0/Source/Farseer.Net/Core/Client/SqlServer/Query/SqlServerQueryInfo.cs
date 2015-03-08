using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Assemble;
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
        /// <summary>
        /// 当前数据库持久化
        /// </summary>
        private readonly IQuery _queryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryProvider">当前数据库持久化</param>
        public SqlServerQueryInfo(IQuery queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public void Query()
        {
            _queryProvider.QueryQueue.Sql = new StringBuilder();

            var strSelectSql = new SelectAssemble(_queryProvider.DbProvider).Execute(_queryProvider.QueryQueue.ExpSelect);
            var strWhereSql = new WhereAssemble(_queryProvider.DbProvider).Execute(_queryProvider.QueryQueue.ExpWhere);
            var strOrderBySql = new OrderByAssemble(_queryProvider.DbProvider).Execute(_queryProvider.QueryQueue.ExpOrderBy);


            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            _queryProvider.QueryQueue.Sql.Append(string.Format("select top 1 {0} ", strSelectSql));

            _queryProvider.QueryQueue.Sql.Append(string.Format("from {0} ", _queryProvider.DbProvider.KeywordAegis(_queryProvider.TableContext.TableName)));

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("where {0} ", strWhereSql));
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                _queryProvider.QueryQueue.Sql.Append(string.Format("orderby {0} ", strOrderBySql));
            }
        }

        public T Query<T>() where T : class, new()
        {
            Query();
            T t;
            using (var reader = _queryProvider.TableContext.Database.GetReader(System.Data.CommandType.Text, _queryProvider.QueryQueue.Sql.ToString()))
            {
                t = reader.ToInfo<T>();
                reader.Close();
            }
            _queryProvider.Init();
            return t;
        }
    }
}