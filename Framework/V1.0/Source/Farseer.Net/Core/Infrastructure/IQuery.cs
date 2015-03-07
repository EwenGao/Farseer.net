using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Data;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库查询持久化
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 数据库
        /// </summary>
        DbExecutor Database { get; set; }
        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        IQueryQueue GroupQueryQueue { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// 将GroupQueryQueue提交到组中，并创建新的GroupQueryQueue
        /// </summary>
        void Execute();

        /// <summary>
        /// 初始化当前查询队列
        /// </summary>
        void Init();
    }
}