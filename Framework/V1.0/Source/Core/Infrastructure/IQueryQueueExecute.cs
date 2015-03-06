using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Core.Infrastructure
{
    public interface IQueryQueueExecute
    {
        StringBuilder Sql { get;}

        /// <summary>
        /// 生成SQL
        /// </summary>
        /// <param name="query"></param>
        void Query(IQuery query);
    }
}
