using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Extend;
using FS.Core.Infrastructure;
using Farseer.Net.Core;

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
            this._dbContext = dbContext;
            QueryProvider = DbFactory.CreateQuery(_dbContext.Database.DataType);
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
            QueryProvider.ExpSelect = QueryProvider.ExpSelect == null ? QueryProvider.ExpSelect = select: Expression.Add(QueryProvider.ExpSelect, select);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            QueryProvider.ExpWhere = QueryProvider.ExpWhere == null ? QueryProvider.ExpWhere = where : Expression.Add(QueryProvider.ExpWhere, where);
            return this;
        }

        public List<TEntity> ToList()
        {
            QueryProvider.GroupQueryQueue.List.Query(QueryProvider);
            return null;
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> desc)
        {
            QueryProvider.ExpOrderBy = QueryProvider.ExpOrderBy == null ? QueryProvider.ExpOrderBy = desc : Expression.Add(QueryProvider.ExpOrderBy, desc);
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> asc)
        {
            QueryProvider.ExpOrderBy = QueryProvider.ExpOrderBy == null ? QueryProvider.ExpOrderBy = asc : Expression.Add(QueryProvider.ExpOrderBy, asc);
            return this;
        }

        public TEntity ToInfo()
        {
            return null;
        }

        public void Update()
        {
        }

        public void Delete()
        {
        }

        public void Insert()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
