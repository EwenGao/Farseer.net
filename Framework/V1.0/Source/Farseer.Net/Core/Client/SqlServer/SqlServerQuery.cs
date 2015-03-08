using System.Collections.Generic;
using System.Data.Common;
using FS.Core.Context;
using FS.Core.Infrastructure;
using System.Linq;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQuery : IQuery
    {
        /// <summary>
        /// 组列表
        /// </summary>
        private List<IQueryQueue> GroupQueryQueueList { get; set; }
        public TableContext TableContext { get; private set; }
        public IQueryQueue QueryQueue { get; set; }
        public DbProvider DbProvider { get; set; }
        public SqlServerQuery(TableContext tableContext)
        {
            TableContext = tableContext;
            GroupQueryQueueList = new List<IQueryQueue>();
            DbProvider = new SqlServerProvider();
            Init();
        }
        public IQueryQueue GetQueryQueue(int index)
        {
            return GroupQueryQueueList[index];
        }

        public IList<DbParameter> Param
        {
            get
            {
                if (GroupQueryQueueList.Count == 0) { return null; }
                var lst = new List<DbParameter>();
                GroupQueryQueueList.Select(o => o.Param).ToList().ForEach(lst.AddRange);
                return lst;
            }
        }

        public void Append()
        {
            if (QueryQueue != null) { GroupQueryQueueList.Add(QueryQueue); }
            Init();
        }

        public int Commit()
        {
            var result = 0;
            foreach (var queryQueue in GroupQueryQueueList)
            {
                result += queryQueue.Execute();
                queryQueue.Dispose();
            }
            GroupQueryQueueList.Clear();
            Init();
            return result;
        }

        public void Init()
        {
            QueryQueue = new SqlServerQueryQueue(GroupQueryQueueList.Count, this);
        }
    }
}
