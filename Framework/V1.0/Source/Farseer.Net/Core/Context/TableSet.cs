using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Context
{
    public class TableSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly TableContext<TEntity> _tableContext;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }

        internal TableSet(TableContext<TEntity> tableContext): this()
        {
            _tableContext = tableContext;
            _tableContext.QueryProvider = DbFactory.CreateQuery(_tableContext);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            //_tableContext.QueryProvider.QueryQueue.ExpSelect = _tableContext.QueryProvider.QueryQueue.ExpSelect == null ? _tableContext.QueryProvider.QueryQueue.ExpSelect = select : Expression.Add(_tableContext.QueryProvider.QueryQueue.ExpSelect, select);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            //_tableContext.QueryProvider.QueryQueue.ExpWhere = _tableContext.QueryProvider.QueryQueue.ExpWhere == null ? _tableContext.QueryProvider.QueryQueue.ExpWhere = where : Expression.Add(_tableContext.QueryProvider.QueryQueue.ExpWhere, where);
            return this;
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            //_tableContext.QueryProvider.QueryQueue.ExpOrderBy = _tableContext.QueryProvider.QueryQueue.ExpOrderBy == null ? _tableContext.QueryProvider.QueryQueue.ExpOrderBy = desc : Expression.Add(_tableContext.QueryProvider.QueryQueue.ExpOrderBy, desc);
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            //_tableContext.QueryProvider.QueryQueue.ExpOrderBy = _tableContext.QueryProvider.QueryQueue.ExpOrderBy == null ? _tableContext.QueryProvider.QueryQueue.ExpOrderBy = asc : Expression.Add(_tableContext.QueryProvider.QueryQueue.ExpOrderBy, asc);
            return this;
        }
        public List<TEntity> ToList()
        {
            return _tableContext.QueryProvider.QueryQueue.List.Query<TEntity>();
        }

        public TEntity ToInfo()
        {
            return _tableContext.QueryProvider.QueryQueue.Info.Query<TEntity>();
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity">实体类</param>
        public TEntity Update(TEntity entity)
        {
            //  获取当前队列索引
            var index = _tableContext.QueryProvider.QueryQueue.Index;
            //  执行SQL
            _tableContext.QueryProvider.QueryQueue.Update.Query(entity);
            // 非合并提交，则直接提交
            if (!_tableContext.IsMergeCommand) { _tableContext.QueryProvider.GetQueryQueue(index).Execute(); }
            return entity;
        }

        public int Delete()
        {
            return _tableContext.QueryProvider.QueryQueue.Delete.Query<TEntity>();
        }

        public TEntity Insert(TEntity entity)
        {
            _tableContext.QueryProvider.QueryQueue.Insert.Query(entity);
            return entity;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
