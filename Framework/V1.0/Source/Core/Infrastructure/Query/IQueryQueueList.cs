using System.Collections.Generic;

namespace FS.Core.Infrastructure.Query
{
    public interface IQueryQueueList : IQueryQueueExecute
    {
        /// <summary>
        /// 生成SQL
        /// </summary>
        List<T> Query<T>();
    }
}
