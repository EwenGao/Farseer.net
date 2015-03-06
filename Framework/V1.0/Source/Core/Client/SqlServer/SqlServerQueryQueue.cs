using FS.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Core.Client.SqlServer
{
    public class SqlServerQueryQueue : IQueryQueue
    {
        public IQueryQueueListExecute List { get { return new SqlServerQueryQueueList(); } }
    }
}
