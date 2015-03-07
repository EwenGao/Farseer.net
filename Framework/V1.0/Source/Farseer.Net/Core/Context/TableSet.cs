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
            QueryProvider = DbFactory.CreateQuery(_dbContext);
        }

        /// <summary>
        /// 数据库查询支持
        /// </summary>
        private IQuery QueryProvider { get; set; }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            QueryProvider.QueryQueue.ExpSelect = QueryProvider.QueryQueue.ExpSelect == null ? QueryProvider.QueryQueue.ExpSelect = select : Expression.Add(QueryProvider.QueryQueue.ExpSelect, select);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            QueryProvider.QueryQueue.ExpWhere = QueryProvider.QueryQueue.ExpWhere == null ? QueryProvider.QueryQueue.ExpWhere = where : Expression.Add(QueryProvider.QueryQueue.ExpWhere, where);
            return this;
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            QueryProvider.QueryQueue.ExpOrderBy = QueryProvider.QueryQueue.ExpOrderBy == null ? QueryProvider.QueryQueue.ExpOrderBy = desc : Expression.Add(QueryProvider.QueryQueue.ExpOrderBy, desc);
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            QueryProvider.QueryQueue.ExpOrderBy = QueryProvider.QueryQueue.ExpOrderBy == null ? QueryProvider.QueryQueue.ExpOrderBy = asc : Expression.Add(QueryProvider.QueryQueue.ExpOrderBy, asc);
            return this;
        }
        public List<TEntity> ToList()
        {
            return QueryProvider.QueryQueue.List.Query<TEntity>();
        }

        public TEntity ToInfo()
        {
            return QueryProvider.QueryQueue.Info.Query<TEntity>();
        }

        public TEntity Update(TEntity entity)
        {
            QueryProvider.QueryQueue.Update.Query(entity);
            return entity;
        }

        public int Delete()
        {
            return QueryProvider.QueryQueue.Delete.Query<TEntity>();
        }

        public TEntity Insert(TEntity entity)
        {
            QueryProvider.QueryQueue.Insert.Query(entity);
            return entity;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
