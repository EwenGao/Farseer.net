using FS.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQuery : IQuery
    {
        public Expression ExpOrderBy { get; set; }

        public Expression ExpSelect { get; set; }

        public Expression ExpWhere { get; set; }

        public IQueryQueue GroupQueryQueue { get; set; }

        private List<IQueryQueue> GroupQueryQueueList { get; set; }

        public string TableName { get; set; }

        public SqlServerQuery()
        {
            GroupQueryQueue = new SqlServerQueryQueue();
            GroupQueryQueueList = new List<IQueryQueue>();
        }
        public void Execute()
        {
            GroupQueryQueueList.Add(GroupQueryQueue);
            GroupQueryQueue = new SqlServerQueryQueue();
        }
    }
}
