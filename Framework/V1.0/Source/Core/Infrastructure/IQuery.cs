using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库查询持久化
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        IQueryQueue GroupQueryQueue { get; set; }
        
        string TableName { get; set; }

        void Execute();

        Expression ExpSelect { get; set; }
        Expression ExpOrderBy { get; set; }
        Expression ExpWhere { get; set; }
    }
}