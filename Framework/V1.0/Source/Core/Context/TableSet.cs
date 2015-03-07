using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Context
{
    public class TableSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TableContext<TEntity> _dbContext;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }

        internal TableSet(TableContext<TEntity> dbContext) : this()
        {
            _dbContext = dbContext;
            QueryProvider = DbFactory.CreateQuery(_dbContext.Database);
            QueryProvider.TableName = _dbContext.TableName;
        }

        /// <summary>
        /// 数据库查询支持
        /// </summary>
        public IQuery QueryProvider { get; set; }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            QueryProvider.GroupQueryQueue.ExpSelect = QueryProvider.GroupQueryQueue.ExpSelect == null ? QueryProvider.GroupQueryQueue.ExpSelect = select : Expression.Add(QueryProvider.GroupQueryQueue.ExpSelect, select);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            QueryProvider.GroupQueryQueue.ExpWhere = QueryProvider.GroupQueryQueue.ExpWhere == null ? QueryProvider.GroupQueryQueue.ExpWhere = where : Expression.Add(QueryProvider.GroupQueryQueue.ExpWhere, where);
            return this;
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            QueryProvider.GroupQueryQueue.ExpOrderBy = QueryProvider.GroupQueryQueue.ExpOrderBy == null ? QueryProvider.GroupQueryQueue.ExpOrderBy = desc : Expression.Add(QueryProvider.GroupQueryQueue.ExpOrderBy, desc);
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            QueryProvider.GroupQueryQueue.ExpOrderBy = QueryProvider.GroupQueryQueue.ExpOrderBy == null ? QueryProvider.GroupQueryQueue.ExpOrderBy = asc : Expression.Add(QueryProvider.GroupQueryQueue.ExpOrderBy, asc);
            return this;
        }
        public List<TEntity> ToList()
        {
            return QueryProvider.GroupQueryQueue.List.Query<TEntity>(QueryProvider);
        }

        public TEntity ToInfo()
        {
            return QueryProvider.GroupQueryQueue.Info.Query<TEntity>(QueryProvider);
        }

        public TEntity Update(TEntity entity)
        {
            return entity;
        }

        public void Delete()
        {
        }

        public TEntity Insert(TEntity entity)
        {
            return entity;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
