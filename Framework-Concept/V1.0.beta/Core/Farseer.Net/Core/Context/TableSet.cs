using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Extend;

namespace FS.Core.Context
{
    public class TableSet<TEntity> : IDisposable where TEntity : class, new()
    {
        /// <summary>
        ///     Select表达式
        /// </summary>
        private List<Expression> ExpSelect;
        /// <summary>
        ///     Where表达式
        /// </summary>
        private Expression<Func<TEntity, bool>> ExpWhere;
        /// <summary>
        ///     表名
        /// </summary>
        //private string TableName;
        /// <summary>
        ///     排序
        /// </summary>
        private Dictionary<Expression, int> ExpSort;
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TableContext<TEntity> _dbContext;

        /// <summary>
        /// 禁止外部实例化
        /// </summary>
        private TableSet() { }

        internal TableSet(TableContext<TEntity> dbContext)
        {
            this._dbContext = dbContext;
            ExpSelect = new List<Expression>();
        }

        public TableSet<TEntity> Desc<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            return this;
        }

        public TableSet<TEntity> Asc<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            return this;
        }

        public List<TEntity> ToList()
        {
            return null;
        }

        public List<TEntity> ToInfo()
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

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public TableSet<TEntity> Select<T>(Expression<Func<TEntity, T>> select)
        {
            //using (Speed.Begin())
            //{
            //    dbBuilder.Select(select);
            //    DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            //}
            if (select != null) { ExpSelect.Add(select); }
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public TableSet<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            //using (Speed.Begin())
            //{
            //    dbBuilder.WhereOr(where);
            //    DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            //}
            ExpWhere = ExpWhere.OrElse(where);
            return this;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
