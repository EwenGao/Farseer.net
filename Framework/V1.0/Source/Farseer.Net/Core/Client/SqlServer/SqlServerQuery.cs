using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQuery : IQuery
    {
        /// <summary>
        /// 组列表
        /// </summary>
        private List<IQueryQueue> GroupQueryQueueList { get; set; }

        public SqlServerQuery(DbExecutor database)
        {
            Database = database;
            Init();
        }

        public DbExecutor Database { get; set; }

        public IQueryQueue GroupQueryQueue { get; set; }

        public string TableName { get; set; }

        public void Execute()
        {
            GroupQueryQueueList.Add(GroupQueryQueue);
            Init();
        }

        public void Init()
        {
            GroupQueryQueue = new SqlServerQueryQueue();
            if (GroupQueryQueueList == null) { GroupQueryQueueList = new List<IQueryQueue>(); }
        }
    }
}
