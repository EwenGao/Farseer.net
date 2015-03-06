using System.Linq.Expressions;
using System.Text;
using FS.Core.Client.SqlServer.Query;
using FS.Core.Infrastructure;
using FS.Core.Infrastructure.Query;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQueryQueue : IQueryQueue
    {
        public Expression ExpOrderBy { get; set; }
        public Expression ExpSelect { get; set; }
        public Expression ExpWhere { get; set; }
        public StringBuilder Sql { get; set; }

        private IQueryQueueList _list;
        public IQueryQueueList List { get { return _list ?? (_list = new SqlServerQueryList(this)); } }


        private IQueryQueueInfo _info;
        public IQueryQueueInfo Info { get { return _info ?? (_info = new SqlServerQueryInfo(this)); } }


        private IQueryQueueInsert _insert;
        public IQueryQueueInsert Insert { get { return _insert ?? (_insert = new SqlServerQueryInsert(this)); } }


        private IQueryQueueUpdate _update;
        public IQueryQueueUpdate Update { get { return _update ?? (_update = new SqlServerQueryUpdate(this)); } }


        private IQueryQueueDelete _delete;
        public IQueryQueueDelete Delete { get { return _delete ?? (_delete = new SqlServerQueryDelete(this)); } }

        public void Dispose()
        {
            _list.Dispose();
            _info.Dispose();
            _insert.Dispose();
            _update.Dispose();
            _delete.Dispose();
        }
    }
}
