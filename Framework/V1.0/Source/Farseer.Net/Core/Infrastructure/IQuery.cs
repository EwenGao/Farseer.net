using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Context;
using FS.Core.Data;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库持久化
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        TableContext TableContext { get; }

        /// <summary>
        /// 当前组查询队列（支持批量提交SQL）
        /// </summary>
        IQueryQueue QueryQueue { get; set; }

        /// <summary>
        /// 根据索引，返回IQueryQueue
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IQueryQueue GetQueryQueue(int index);
        /// <summary>
        /// 数据库提供者
        /// </summary>
        DbProvider DbProvider { get; set; }

        /// <summary>
        /// 将GroupQueryQueue提交到组中，并创建新的GroupQueryQueue
        /// </summary>
        void Commit();

        /// <summary>
        /// 初始化当前查询队列
        /// </summary>
        void Init();
    }
}