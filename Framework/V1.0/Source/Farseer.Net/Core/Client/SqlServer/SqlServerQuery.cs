using System.Collections.Generic;
using FS.Core.Context;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQuery : IQuery
    {
        /// <summary>
        /// 组列表
        /// </summary>
        private List<IQueryQueue> GroupQueryQueueList { get; set; }

        public SqlServerQuery(TableContext tableContext)
        {
            TableContext = tableContext;
            Init();
            DbProvider = new SqlServerProvider();
        }

        public TableContext TableContext { get; private set; }
        public IQueryQueue QueryQueue { get; set; }
        public DbProvider DbProvider { get; set; }

        public void Execute()
        {
            GroupQueryQueueList.Add(QueryQueue);
            Init();
        }

        public void Init()
        {
            QueryQueue = new SqlServerQueryQueue(this);
            if (GroupQueryQueueList == null) { GroupQueryQueueList = new List<IQueryQueue>(); }
        }
    }
}
