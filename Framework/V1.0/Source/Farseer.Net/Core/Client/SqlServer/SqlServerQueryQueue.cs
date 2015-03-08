using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Query;
using FS.Core.Infrastructure;
using FS.Core.Infrastructure.Query;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQueryQueue : IQueryQueue
    {
        private readonly IQuery _queryProvider;
        public Expression ExpOrderBy { get; set; }
        public int Index { get; set; }
        public Expression ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public StringBuilder Sql { get; set; }
        public IList<DbParameter> Param { get; set; }

        public SqlServerQueryQueue(int index, IQuery queryProvider)
        {
            Index = index;
            _queryProvider = queryProvider;
        }

        private IQueryQueueList _list;
        public IQueryQueueList List { get { return _list ?? (_list = new SqlServerQueryList(_queryProvider)); } }


        private IQueryQueueInfo _info;
        public IQueryQueueInfo Info { get { return _info ?? (_info = new SqlServerQueryInfo(_queryProvider)); } }


        private IQueryQueueInsert _insert;
        public IQueryQueueInsert Insert { get { return _insert ?? (_insert = new SqlServerQueryInsert(_queryProvider)); } }


        private IQueryQueueUpdate _update;
        public IQueryQueueUpdate Update { get { return _update ?? (_update = new SqlServerQueryUpdate(_queryProvider)); } }


        private IQueryQueueDelete _delete;
        public IQueryQueueDelete Delete { get { return _delete ?? (_delete = new SqlServerQueryDelete(_queryProvider)); } }

        public int Execute()
        {
            if (Sql.Length < 1) { return 0; }
            return _queryProvider.TableContext.Database.ExecuteNonQuery(CommandType.Text, Sql.ToString(), Param == null ? null : ((List<DbParameter>)Param).ToArray());
        }

        public void Dispose()
        {
            Sql.Clear();
            ExpOrderBy = null;
            ExpSelect = null;
            ExpWhere = null;
            Sql = null;
            _list = null;
            _info = null;
            _insert = null;
            _update = null;
            _delete = null;
        }
    }
}
