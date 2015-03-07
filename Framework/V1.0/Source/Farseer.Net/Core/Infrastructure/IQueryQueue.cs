using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Infrastructure.Query;

namespace FS.Core.Infrastructure
{
    public interface IQueryQueue : IDisposable
    {
        /// <summary>
        /// 查询字段的表达式树
        /// </summary>
        Expression ExpSelect { get; set; }
        /// <summary>
        /// 排序字段的表达式树
        /// </summary>
        Expression ExpOrderBy { get; set; }
        /// <summary>
        /// 条件字段的表达式树
        /// </summary>
        Expression ExpWhere { get; set; }
        /// <summary>
        /// 当前生成的SQL
        /// </summary>
        StringBuilder Sql { get; set; }
        IQueryQueueList List { get; }
        IQueryQueueInfo Info { get; }
        IQueryQueueInsert Insert { get; }
        IQueryQueueUpdate Update { get; }
        IQueryQueueDelete Delete { get; }
    }
}
