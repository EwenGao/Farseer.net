using FS.Core.Bean;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Extend;
using System.Data;
using FS.Utils.Common;
using FS.UI;
using System.Data.Common;
using EntityFramework.Extensions;
using System.Collections;

namespace FS.Core.Model
{
    public class BaseModel<TInfo> : BaseInfo where TInfo : BaseInfo, new()
    {
        /// <summary>
        ///     查询结果
        /// </summary>
        public static DataResult<TInfo> DataResult { get; set; }
        private static DbContext<TInfo> _DbContext;
        public static DbContext<TInfo> DbContext
        {
            get { return _DbContext ?? (_DbContext = DbContext<TInfo>.CreateInstance()); }
        }
        public static Bean<TInfo> Data
        {
            get { return new Bean<TInfo>(DbContext<TInfo>.CreateInstance()); }
        }
    }

    public class Bean<TInfo> where TInfo : BaseInfo, new()
    {
        /// <summary>
        /// Entityframework Query
        /// </summary>
        private IQueryable<TInfo> Query;
        /// <summary>
        ///     Select表达式
        /// </summary>
        private List<Expression> ExpSelect;
        /// <summary>
        ///     Where表达式
        /// </summary>
        private Expression<Func<TInfo, bool>> ExpWhere;
        /// <summary>
        ///     表名
        /// </summary>
        //private string TableName;
        /// <summary>
        ///     排序
        /// </summary>
        private Dictionary<Expression, int> ExpSort;
        /// <summary>
        ///     数据执行测试
        /// </summary>
        private readonly SpeedTest Speed;
        /// <summary>
        ///     返回执行结果
        /// </summary>
        public DataResult<TInfo> DataResult;
        /// <summary>
        /// DbContext
        /// </summary>
        public DbContext<TInfo> DbContext;

        public Bean(DbContext<TInfo> db)
        {
            DbContext = db;
            Speed = new SpeedTest();
            Query = db.Info.AsQueryable<TInfo>();
            DataResult = new DataResult<TInfo>();
            ExpSelect = new List<Expression>();
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public Bean<TInfo> Where(Expression<Func<TInfo, bool>> where)
        {
            Query = Query.Where(where);
            ExpWhere = ExpWhere.AndAlso(where);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public Bean<TInfo> WhereOr(Expression<Func<TInfo, bool>> where)
        {
            Query = Query.Where(where);
            ExpWhere = ExpWhere.OrElse(where);
            return this;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public Bean<TInfo> Select<T>(Expression<Func<TInfo, T>> select)
        {
            foreach (var item in ExpSelect)
            {
               //Query.Select(select).
            }
            if (select != null) { ExpSelect.Add(select); }
            return this;
        }

        public struct OrderModelField
        {
            public string propertyName { get; set; }
            public bool IsDESC { get; set; }
        }

        public IList GetAllEntity<T>(Expression<Func<TInfo, bool>> condition, Expression<Func<TInfo, T>> select, int pageIndex, int pageSize, out long total, params OrderModelField[] orderByExpression)
        {
            //条件过滤
            var query = Query.Where(condition);

            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(TInfo), "o");

            if (orderByExpression != null && orderByExpression.Length > 0)
            {
                for (int i = 0; i < orderByExpression.Length; i++)
                {
                    //根据属性名获取属性
                    var property = typeof(TInfo).GetProperty(orderByExpression[i].propertyName);
                    //创建一个访问属性的表达式
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    string OrderName = orderByExpression[i].IsDESC ? "OrderByDescending" : "OrderBy";

                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(TInfo), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                    query = (IQueryable<TInfo>)query.Provider.CreateQuery(resultExp);
                }
            }

            total = query.Count();
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public Expression<Func<TSource, TResult>> CreateSelecter<TSource, TResult>(string colNumber, string colName)
        {
            Expression<Func<TSource, TResult>> selector = null;

            //(rec)
            ParameterExpression param = Expression.Parameter(typeof(TSource), "rec");
            //new ParadigmSearchListData 
            var v0 = Expression.New(typeof(TResult));
            //Number
            var v1 = typeof(TResult).GetProperty("Number");
            //rec.NUMBER
            var v2 = Expression.Property(param, typeof(TSource).GetProperty(colNumber));
            //Name
            var v3 = typeof(TResult).GetProperty("Name");
            //rec.RES_NAME
            var v4 = Expression.Property(param, typeof(TSource).GetProperty(colName));

            Expression body = Expression.MemberInit(v0,
                //{ Number = rec.NUMBER, Name = rec.RES_NAME }
                new MemberBinding[] 
                {
                    //Number = rec.NUMBER
                    Expression.Bind(v1, v2),
                    //Name = rec.RES_NAME
                    Expression.Bind(v3, v4)
                });

            selector = (Expression<Func<TSource, TResult>>)Expression.Lambda(body, param);

            return selector;
        }
        public IQueryable<TInfo> GetListDataSelect<T>(Expression<Func<TInfo, T>> selector)
        {
            //依据IQueryable数据源构造一个查询
            IQueryable<TInfo> custs = Query;

            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(TInfo), "o");                               // o=>
            var property = Expression.Property(parameter, typeof(TInfo).GetProperty("ID"));        // ID
            var pred = Expression.Lambda(property, parameter);                                     // o=>o.ID

            //组建表达式树:Select(c=>c.ContactName)
            var expr = Expression.Call(typeof(Queryable), "Select", new Type[] { typeof(TInfo), typeof(T) }, Expression.Constant(custs), selector);

            //使用表达式树来生成动态查询
            Query.Provider.CreateQuery(expr);

            return Query;
        }

        /// <summary>
        ///     升序
        /// </summary>
        /// <param name="sort">升序</param>
        public Bean<TInfo> Asc(Expression<Func<TInfo, object>> sort)
        {
            if (sort != null) { ExpSort.Add(sort, 0); }
            return this;
        }

        /// <summary>
        ///     降序
        /// </summary>
        /// <param name="sort">降序</param>
        public Bean<TInfo> Desc(Expression<Func<TInfo, object>> sort)
        {
            if (sort != null) { ExpSort.Add(sort, 1); }
            return this;
        }

        ///// <summary>
        /////     返回筛选后的列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="db">事务</param>
        //public List<T> ToSelectList<T>(Expression<Func<TInfo, T>> select)
        //{
        //    return ToSelectList<T>(0, select);
        //}

        // /// <summary>
        // ///     返回筛选后的列表
        // /// </summary>
        // /// <param name="select">字段选择器</param>
        ///// <param name="db">事务</param>
        //public List<T> ToSelectList<T>(int top, Expression<Func<TInfo, T>> select)
        //{
        //    return Select(select).ToList(top).Select(select.Compile()).ToList();
        //}

        ///// <summary>
        /////     返回筛选后的列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="db">事务</param>
        //public List<T> ToSelectList<T>(int top, Expression<Func<TInfo, T?>> select) where T : struct
        //{
        //    return Select(select).ToList(top).Select(select.Compile()).ToList().ConvertType<List<T>>();
        //}

        ///// <summary>
        /////     返回筛选后的列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="IDs">o => IDs.Contains(o.ID)</param>
        ///// <param name="db">事务</param>
        //public List<T> ToSelectList<T>(List<int> IDs, Expression<Func<TInfo, T>> select)
        //{
        //    return Where(o => IDs.Contains(o.ID)).ToSelectList<T>(select);
        //}

        ///// <summary>
        /////     返回筛选后的列表
        ///// </summary>
        ///// <param name="select">字段选择器</param>
        ///// <param name="IDs">o => IDs.Contains(o.ID)</param>
        ///// <param name="db">事务</param>
        //public List<T> ToSelectList<T>(List<int> IDs, int top, Expression<Func<TInfo, T>> select)
        //{
        //    return Where(o => IDs.Contains(o.ID)).ToSelectList<T>(top, select);
        //}

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="info">已赋值的实体</param>
        /// <param name="db">可传入事务的db</param>
        public bool Insert(params TInfo[] infos)
        {
            DbContext.Info.AddRange(infos);
            var result = DataSpeed(() => { return DbContext.SaveChanges(); });
            return result > 0;
        }

        /// <summary>
        ///     更新数据
        /// </summary>
        /// <param name="info">已赋值的实体</param>
        /// <param name="db">可传入事务的db</param>
        public bool Update(TInfo info)
        {
            IQueryable<TInfo> query;

            // 默认不带条件，并且Info.ID有值时，则自动修改当前ID
            if (ExpWhere == null && info.ID == 0) { query = DbContext.Info.Where(o => o.ID == info.ID); }
            else { query = DbContext.Info.Where(ExpWhere); }

            var result = DataSpeed(() => { return DbContext.Info.Update(query, o => info); });
            return result > 0;
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool Update(int? ID, TInfo info)
        {
            return Where(o => o.ID == ID).Update(info);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool Update(List<int> IDs, TInfo info)
        {
            return Where(o => IDs.Contains(o.ID)).Update(info);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        public int Count()
        {
            var query = DbContext.Info.Where(ExpWhere);
            var result = DataSpeed(() => { return query.Count(); });
            return result;
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public int Count(int ID)
        {
            return Where(o => o.ID == ID).Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public int Count(List<int> IDs)
        {
            return Where(o => IDs.Contains(o.ID)).Count();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public bool IsHaving()
        {
            return Count() > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool IsHaving(int ID)
        {
            return Where(o => o.ID == ID).IsHaving();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool IsHaving(List<int> IDs)
        {
            return Where(o => IDs.Contains(o.ID)).IsHaving();
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToInfo()
        {
            var result = DataSpeed(() => { return Query.FirstOrDefault(); });
            return result;
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public TInfo ToInfo(int ID)
        {
            return Where(o => o.ID == ID).ToInfo();
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToNextInfo(int? ID)
        {
            return Where(o => o.ID > ID).Asc(o => o.ID).ToInfo();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToPreviousInfo(int? ID)
        {
            return Where(o => o.ID < ID).Desc(o => o.ID).ToInfo();
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="recordCount">返回数据总数</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        /// <param name="db">可传入事务的db</param>
        //public DataTable ToTable(out int recordCount, int pageSize, int pageIndex)
        //{
        //    #region 计算总页数

        //    recordCount = Count(db);
        //    var allCurrentPage = 1;

        //    if (pageIndex < 1)
        //    {
        //        pageIndex = 1;
        //    }
        //    if (pageSize < 0)
        //    {
        //        pageSize = 0;
        //    }
        //    if (pageSize != 0)
        //    {
        //        allCurrentPage = (recordCount / pageSize);
        //        allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
        //        allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
        //    }
        //    if (pageIndex > allCurrentPage)
        //    {
        //        pageIndex = allCurrentPage;
        //    }

        //    #endregion

        //    return DataSpeed(
        //        () => { return dbBuilder.ToTable(pageSize, pageIndex); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value; }
        //        );
        //}

        ///// <summary>
        /////     获取数据列表
        ///// </summary>
        ///// <param name="top">Top数量</param>
        ///// <param name="db">可传入事务的db</param>
        //public DataTable ToTable(int top = 0)
        //{
        //    return DataSpeed(
        //        () => { return dbBuilder.ToTable(top); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value; }
        //        );
        //}

        ///// <summary>
        /////     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        ///// </summary>
        ///// <param name="pageIndex">分页索引，为1时，使用Top方法</param>
        ///// <param name="pageSize">为0时，获取所有数据</param>
        ///// <param name="db">可传入事务的db</param>
        //public DataTable ToTable(int pageSize, int pageIndex)
        //{
        //    return DataSpeed(
        //        () => { return dbBuilder.ToTable(pageSize, pageIndex); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value; }
        //        );
        //}

        ///// <summary>
        /////     获取随机列表
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="top">Top数量</param>
        //public DataTable ToDataTableByRand(int top = 0)
        //{
        //    return DataSpeed(
        //        () => { return dbBuilder.ToTableByRand(top); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value; }
        //        );
        //}

        ///// <summary>
        /////     获取数据列表
        ///// </summary>
        ///// <param name="recordCount">返回数据总数</param>
        ///// <param name="pageSize">每页大小</param>
        ///// <param name="pageIndex">索引</param>
        ///// <param name="db">可传入事务的db</param>
        //public List<TInfo> ToList(out int recordCount, int pageSize, int pageIndex)
        //{
        //    #region 计算总页数

        //    recordCount = Count(db);
        //    var allCurrentPage = 1;

        //    if (pageIndex < 1)
        //    {
        //        pageIndex = 1;
        //    }
        //    if (pageSize < 0)
        //    {
        //        pageSize = 0;
        //    }
        //    if (pageSize != 0)
        //    {
        //        allCurrentPage = (recordCount / pageSize);
        //        allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
        //        allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
        //    }
        //    if (pageIndex > allCurrentPage)
        //    {
        //        pageIndex = allCurrentPage;
        //    }

        //    #endregion

        //    var val = DataSpeed(
        //        () => { return dbBuilder.ToTable(pageSize, pageIndex); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ToList<TInfo>(); }
        //        );

        //    return val;
        //}

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top">Top数量</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(int top = 0)
        {
            var result = DataSpeed(() => { return Query.ToList(); });
            return result;
        }

        ///// <summary>
        /////     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        ///// </summary>
        ///// <param name="pageIndex">分页索引，为1时，使用Top方法</param>
        ///// <param name="pageSize">为0时，获取所有数据</param>
        ///// <param name="db">可传入事务的db</param>
        //public List<TInfo> ToList(int pageSize, int pageIndex)
        //{
        //    return DataSpeed(
        //        () => { return dbBuilder.ToTable(pageSize, pageIndex); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ToList<TInfo>(); }
        //        );
        //}

        ///// <summary>
        /////     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        ///// </summary>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        ///// <param name="db">可传入事务的db</param>
        //public List<TInfo> ToList(List<int> IDs)
        //{
        //    return Where(o => IDs.Contains(o.ID)).ToList(0);
        //}

        /// <summary>
        ///     通用的分页方法(多条件)
        /// </summary>
        /// <param name="rpt">Repeater带分页控件</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(Repeater rpt)
        {
            var result = DataSpeed(() => { return Query.ToList(); });
            return result;

            //int recordCount;
            //var lst = ToList(out recordCount, rpt.PageSize, rpt.PageIndex);
            //rpt.PageCount = recordCount;

            //return lst;
        }

        ///// <summary>
        /////     获取所有列表
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        //public List<TInfo> ToList()
        //{
        //    return ToList(0);
        //}

        ///// <summary>
        /////     获取随机列表
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="top">Top数量</param>
        //public List<TInfo> ToListByRand(int top = 0)
        //{
        //    return DataSpeed(
        //        () => { return dbBuilder.ToTableByRand(top); },
        //        (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ToList<TInfo>(); }
        //        );
        //}

        ///// <summary>
        /////     获取Sum总和
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="select">字段选择器</param>
        ///// <param name="defValue">默认值</param>
        //public T GetSum<T>(Expression<Func<TInfo, T>> select, T defValue = default(T))
        //{
        //    Select(select);
        //    return DataSpeed(
        //        () =>
        //        {
        //            return dbBuilder.GetSum();
        //        },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ConvertType(defValue); }
        //        );
        //}

        ///// <summary>
        /////     获取Sum总和
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="defValue">默认值</param>
        //public T GetSum<T>(T defValue)
        //{
        //    return GetSum(null, defValue);
        //}

        ///// <summary>
        /////     获取Max最大值
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="select">字段选择器</param>
        ///// <param name="defValue">默认值</param>
        //public T GetMax<T>(Expression<Func<TInfo, T>> select, T defValue = default(T))
        //{
        //    Select(select);
        //    return DataSpeed(
        //        () =>
        //        {
        //            return dbBuilder.GetMax();
        //        },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ConvertType(defValue); }
        //        );
        //}

        ///// <summary>
        /////     获取Max最大值
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="defValue">默认值</param>
        //public T GetMax<T>(T defValue)
        //{
        //    return GetMax(null, defValue);
        //}

        ///// <summary>
        /////     获取Min最小值
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="select">字段选择器</param>
        ///// <param name="defValue">默认值</param>
        //public T GetMin<T>(Expression<Func<TInfo, T>> select, T defValue = default(T))
        //{
        //    Select(select);
        //    return DataSpeed(
        //        () =>
        //        {
        //            return dbBuilder.GetMin();
        //        },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ConvertType(defValue); }
        //        );
        //}

        ///// <summary>
        /////     获取Min最小值
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="defValue">默认值</param>
        //public T GetMin<T>(T defValue)
        //{
        //    return GetMin(null, defValue);
        //}

        ///// <summary>
        /////     获取单个值
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="defValue">默认值</param>
        ///// <param name="select">字段选择器</param>
        ///// <param name="db">可传入事务的db</param>
        //public T GetValue<T>(Expression<Func<TInfo, T>> select, T defValue = default(T))
        //{
        //    Select(select);
        //    return DataSpeed(
        //        () =>
        //        {
        //            return dbBuilder.GetValue();
        //        },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
        //        (value) => { return value.ConvertType<T>(); }
        //        );
        //}

        ///// <summary>
        /////     获取单个值
        ///// </summary>
        ///// <typeparam name="T">返回结果</typeparam>
        ///// <param name="defValue">默认值</param>
        ///// <param name="db">可传入事务的db</param>
        //public T GetValue<T>(T defValue)
        //{
        //    return GetValue((Expression<Func<TInfo, T>>)null, defValue);
        //}

        ///// <summary>
        /////     获取单个字段的数据
        ///// </summary>
        ///// <typeparam name="T">值类型变量</typeparam>
        ///// <param name="defValue">为null时默认值</param>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        ///// <param name="select">字段选择器</param>
        //public T GetValue<T>(int? ID, Expression<Func<TInfo, T>> select, T defValue = default(T))
        //{
        //    return Where(o => o.ID == ID).GetValue(select, defValue);
        //}

        ///// <summary>
        /////     获取单个字段的数据
        ///// </summary>
        ///// <typeparam name="T">值类型变量</typeparam>
        ///// <param name="defValue">为null时默认值</param>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        //public T GetValue<T>(int? ID, T defValue = default(T))
        //{
        //    return GetValue(ID, null, defValue);
        //}

        ///// <summary>
        /////     更新单个字段值
        ///// </summary>
        ///// <typeparam name="T">更新的值类型</typeparam>
        ///// <param name="fieldValue">要更新的值</param>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="ID">o => o.ID == ID</param>
        //public bool UpdateValue<T>(int? ID, Expression<Func<TInfo, T>> select, T fieldValue) where T : struct
        //{
        //    return Where(o => o.ID == ID).UpdateValue(select, fieldValue);
        //}

        ///// <summary>
        /////     更新单个字段值
        ///// </summary>
        ///// <typeparam name="T">更新的值类型</typeparam>
        ///// <param name="fieldValue">要更新的值</param>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="ID">o => o.ID == ID</param>
        //public bool UpdateValue<T>(int? ID, T fieldValue) where T : struct
        //{
        //    return UpdateValue(ID, null, fieldValue);
        //}

        ///// <summary>
        /////     更新单个字段值
        ///// </summary>
        ///// <typeparam name="T">更新的值类型</typeparam>
        ///// <param name="fieldValue">要更新的值</param>
        ///// <param name="db">可传入事务的db</param>
        //public bool UpdateValue<T>(Expression<Func<TInfo, T>> select, T fieldValue) where T : struct
        //{
        //    Select(select);

        //    return DataSpeed(
        //        () => { return dbBuilder.UpdateValue(fieldValue); },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
        //        (value) => { return value > 0; }
        //        );
        //}

        ///// <summary>
        /////     更新单个字段值
        ///// </summary>
        ///// <typeparam name="T">更新的值类型</typeparam>
        ///// <param name="fieldValue">要更新的值</param>
        ///// <param name="db">可传入事务的db</param>
        //public bool UpdateValue<T>(T fieldValue) where T : struct
        //{
        //    return UpdateValue((Expression<Func<TInfo, T>>)null, fieldValue);
        //}

        ///// <summary>
        /////     删除数据
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        //public bool Delete()
        //{
        //    return DataSpeed(
        //        () => { return dbBuilder.Delete(); },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
        //        (value) => { return value > 0; }
        //        );
        //}

        ///// <summary>
        /////     删除数据
        ///// </summary>
        ///// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        ///// <param name="db">可传入事务的db</param>
        //public bool Delete(int? ID)
        //{
        //    return Where(o => o.ID == ID).Delete(db);
        //}

        ///// <summary>
        /////     删除数据
        ///// </summary>
        ///// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        ///// <param name="db">可传入事务的db</param>
        //public bool Delete(List<int> IDs)
        //{
        //    return Where(o => IDs.Contains(o.ID)).Delete(db);
        //}

        ///// <summary>
        /////     重置标识
        ///// </summary>
        ///// <param name="db">可传入事务的db</param>
        //public void ResetIdentity()
        //{
        //    DataSpeed(
        //        () => { return dbBuilder.ResetIdentity(); },
        //        (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
        //        (value) => { return value; }
        //        );
        //}

        ///// <summary>
        /////     复制数据
        ///// </summary>
        ///// <param name="actInfo">对新职的赋值</param>
        ///// <param name="db">可传入事务的db</param>
        //public void Copy(Action<TInfo> actInfo = null)
        //{
        //    var lst = ToList(db);
        //    foreach (var info in lst)
        //    {
        //        info.ID = null;
        //        if (actInfo != null) actInfo(info);
        //        Insert(info);
        //    }
        //}

        ///// <summary>
        /////     复制数据
        ///// </summary>
        ///// <param name="actInfo">对新职的赋值</param>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="ID">o => o.ID == ID</param>
        //public void Copy(int? ID, Action<TInfo> actInfo = null)
        //{
        //    Where(o => o.ID == ID);
        //    Copy(actInfo);
        //}

        ///// <summary>
        /////     复制数据
        ///// </summary>
        ///// <param name="actInfo">对新职的赋值</param>
        ///// <param name="db">可传入事务的db</param>
        ///// <param name="IDs">o => IDs.Contains(o.ID)</param>
        //public void Copy(List<int> IDs, Action<TInfo> actInfo = null)
        //{
        //    Where(o => IDs.Contains(o.ID));
        //    Copy(actInfo);
        //}

        ///// <summary>
        ///// 指量插入数据（仅支付Sql Server)
        ///// </summary>
        ///// <param name="tableName">表名</param>
        ///// <param name="dt">数据</param>
        //public void SqlBulkCopy(List<TInfo> lst)
        //{
        //    var dt = lst.ToDataTable();
        //    DataSpeed(
        //        () => { return ""; },
        //        (sql, Parms) => { (db ?? DbExtor).ExecuteSqlBulkCopy(TableName, dt); return true; },
        //        (value) => { return value; }
        //        );
        //}

        ///// <summary>
        ///// 指量插入数据（仅支付Sql Server)
        ///// </summary>
        ///// <param name="tableName">表名</param>
        ///// <param name="dt">数据</param>
        //public void SqlBulkCopy(DataTable dt)
        //{
        //    DataSpeed(
        //        () => { return ""; },
        //        (sql, Parms) => { (db ?? DbExtor).ExecuteSqlBulkCopy(TableName, dt); return true; },
        //        (value) => { return value; }
        //        );
        //}

        /// <summary>
        ///     计算执行使用的时间
        /// </summary>
        /// <typeparam name="T">返回值</typeparam>
        /// <typeparam name="T2">数据库中返回的值</typeparam>
        /// <param name="actBuilderSql">生成Sql步骤</param>
        /// <param name="actGetData">获取数据步骤</param>
        /// <param name="actParseData">转换数据类型步骤</param>
        private T DataSpeed<T>(Func<T> func)
        {
            using (Speed.Begin())
            {
                //DataResult.ParmsList = dbBuilder.ParamsList;
                //DataResult.SqlBuildTime += Speed.Result.Timer.Elapsed.TotalMilliseconds;

                // 获取数据
                Speed.Result.Timer.Restart();
                var result = func();
                DataResult.OperateDataTime += Speed.Result.Timer.Elapsed.TotalMilliseconds;

                DataResult.Sql = DbContext.Info.ToString();

                // 转换数据
                //Speed.Result.Timer.Restart();
                //var v = actParseData(value);
                //DataResult.ToModelTime += Speed.Result.Timer.Elapsed.TotalMilliseconds;

                // 记录执行过程
                SqlTemplate.Output(DataResult);
                return result;
            }
        }
    }
}
