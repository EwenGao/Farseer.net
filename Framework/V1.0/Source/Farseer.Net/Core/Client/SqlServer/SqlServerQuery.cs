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
            GroupQueryQueueList = new List<IQueryQueue>();
            DbProvider = new SqlServerProvider();
            Init();
        }

        public TableContext TableContext { get; private set; }
        public IQueryQueue QueryQueue { get; set; }
        public IQueryQueue GetQueryQueue(int index)
        {
            return GroupQueryQueueList[index];
        }
        public DbProvider DbProvider { get; set; }
        public void Commit()
        {
            if (QueryQueue != null) { GroupQueryQueueList.Add(QueryQueue); }
            Init();
        }
        public void Init()
        {
            QueryQueue = new SqlServerQueryQueue(GroupQueryQueueList.Count, this);
        }
    }
}
