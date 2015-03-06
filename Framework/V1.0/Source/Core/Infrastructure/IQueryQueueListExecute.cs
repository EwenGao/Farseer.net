using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Core.Infrastructure
{
    public interface IQueryQueueListExecute : IQueryQueueExecute
    {
                /// <summary>
        /// 生成SQL
        /// </summary>
        /// <param name="query"></param>
        List<T> Query<T>(IQuery query);
    }
}
