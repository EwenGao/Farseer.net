using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using FS.Configs;
using FS.Core.Bean;
using FS.Core.Data;
using FS.Extend;
using FS.Mapping.Table;
using FS.UI;
using FS.Utils.Common;
using FS.Utils.Web;

namespace FS.Core.Model
{
    /// <summary>
    ///     逻辑层基类工具
    /// </summary>
    /// <typeparam name="TInfo">实体类</typeparam>
    [Serializable]
    public class BaseModel<TInfo> : ModelInfo where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     查询结果
        /// </summary>
        public static DataResult<TInfo> DataResult { get; set; }

        /// <summary>
        ///     数据库持久化
        /// </summary>
        public static Bean<TInfo> Data
        {
            get
            {
                var bean = new Bean<TInfo>();
                DataResult = bean.DataResult;
                return bean;
            }
        }

        /// <summary>
        ///     数据库持久化
        /// </summary>
        public static CacheDataBean<TInfo> Cache
        {
            get
            {
                var bean = new CacheDataBean<TInfo>();
                DataResult = bean.DataResult;
                return bean;
            }
        }

        #region 提交过来的内容转化成为实体类
        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        /// <param name="url">跳转地址</param>
        public static TInfo Form(Action<string, string> tip = null, string url = "", string prefix = "hl")
        {
            return Req.Fill<TInfo>(HttpContext.Current.Request.Form, tip, url, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TInfo Form(Action<Dictionary<string, List<string>>> tip, string prefix = "hl")
        {
            return Req.Fill<TInfo>(HttpContext.Current.Request.Form, tip, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TInfo Form(out Dictionary<string, List<string>> dicError, string prefix = "hl")
        {
            return Req.Fill<TInfo>(HttpContext.Current.Request.Form, out dicError, prefix);
        }

        /// <summary>
        ///     把Request.QueryString提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        /// <param name="url">跳转地址</param>
        public static TInfo QueryString(Action<string, string> tip = null, string url = "", string prefix = "hl")
        {
            return Req.Fill<TInfo>(HttpContext.Current.Request.QueryString, tip, url, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="tip">弹出框事务委托</param>
        public static TInfo QueryString(Action<Dictionary<string, List<string>>> tip, string prefix = "hl")
        {
            return Req.Fill<TInfo>(HttpContext.Current.Request.QueryString, tip, prefix);
        }

        /// <summary>
        ///     把Request.Form提交过来的内容转化成为实体类
        /// </summary>
        /// <param name="prefix">控件前缀</param>
        /// <param name="dicError">返回错误消息,key：属性名称；value：错误消息</param>
        public static TInfo QueryString(out Dictionary<string, List<string>> dicError, string prefix = "hl")
        {
            return Req.Fill<TInfo>(HttpContext.Current.Request.QueryString, out dicError, prefix);
        }

        #endregion
    }

    /// <summary>
    /// Bean
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    public class Bean<TInfo> where TInfo : ModelInfo, new()
    {
        private readonly DbExecutor DbExtor;

        /// <summary>
        ///     实体映射
        /// </summary>
        private readonly TableMap Map = typeof(TInfo);

        /// <summary>
        ///     数据执行测试
        /// </summary>
        private readonly SpeedTest Speed = new SpeedTest();

        /// <summary>
        ///     数据库Sql生成
        /// </summary>
        private readonly DbBuilder<TInfo> dbBuilder;

        /// <summary>
        ///     Where表达式
        /// </summary>
        private Expression<Func<TInfo, bool>> ExpWhere;
        /// <summary>
        /// 表名
        /// </summary>
        private string TableName;

        /// <summary>
        ///     Select表达式
        /// </summary>
        List<Expression> ExpSelect;

        Dictionary<Expression, int> ExpSort;

        /// <summary>
        ///     兼容Qyn.Factory项目
        /// </summary>
        /// <param name="dbIndex">数据库配置索引</param>
        /// <param name="tableName">表名称</param>
        public Bean(int dbIndex, string tableName = "")
        {
            DbInfo dbInfo = dbIndex;
            TableName = tableName.IsNullOrEmpty() ? Map.ClassInfo.Name : tableName;
            dbBuilder = DbFactory.CreateDbBuilder<TInfo>(TableName);

            DbExtor = new DbExecutor(dbInfo.DataType, DbFactory.CreateConnString(dbIndex), dbInfo.CommandTimeout);
            DataResult = new DataResult<TInfo>();
            ExpSelect = new List<Expression>();
            ExpSort = new Dictionary<Expression, int>();
        }

        /// 
        /// <summary>
        ///     兼容Qyn.Factory项目
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="commandTimeout">命令执时超时时间</param>
        /// <param name="tableName">表名称</param>
        public Bean(DataBaseType dbType, string connetionString, int commandTimeout, string tableName = "")
        {
            TableName = tableName.IsNullOrEmpty() ? Map.ClassInfo.Name : tableName;
            dbBuilder = DbFactory.CreateDbBuilder<TInfo>(TableName);

            DbExtor = new DbExecutor(dbType, connetionString, commandTimeout);
            DataResult = new DataResult<TInfo>();
            ExpSelect = new List<Expression>();
            ExpSort = new Dictionary<Expression, int>();
        }

        /// <summary>
        ///     默认使用
        /// </summary>
        /// <param name="tableName">表名称</param>
        public Bean(string tableName = "")
        {
            TableName = tableName.IsNullOrEmpty() ? Map.ClassInfo.Name : tableName;
            dbBuilder = DbFactory.CreateDbBuilder<TInfo>(TableName);
            DbExtor = DbFactory.CreateDbExecutor<TInfo>(IsolationLevel.Unspecified);
            DataResult = new DataResult<TInfo>();
            ExpSelect = new List<Expression>();
            ExpSort = new Dictionary<Expression, int>();
        }

        /// <summary>
        ///     返回执行结果
        /// </summary>
        public DataResult<TInfo> DataResult { get; set; }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public Bean<TInfo> Where(Expression<Func<TInfo, bool>> where)
        {
            using (Speed.Begin())
            {
                dbBuilder.Where(where);
                DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            }
            ExpWhere = ExpWhere.AndAlso(where);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public Bean<TInfo> WhereOr(Expression<Func<TInfo, bool>> where)
        {
            using (Speed.Begin())
            {
                dbBuilder.WhereOr(where);
                DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            }
            ExpWhere = ExpWhere.OrElse(where);
            return this;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public Bean<TInfo> Select<T>(Expression<Func<TInfo, T>> select)
        {
            using (Speed.Begin())
            {
                dbBuilder.Select(select);
                DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            }
            if (select != null) { ExpSelect.Add(select); }
            return this;
        }

        /// <summary>
        ///     升序
        /// </summary>
        /// <param name="sort">升序</param>
        public Bean<TInfo> Asc(Expression<Func<TInfo, object>> sort)
        {
            using (Speed.Begin())
            {
                dbBuilder.Asc(sort);
                DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            }
            if (sort != null) { ExpSort.Add(sort, 0); }
            return this;
        }

        /// <summary>
        ///     降序
        /// </summary>
        /// <param name="sort">降序</param>
        public Bean<TInfo> Desc(Expression<Func<TInfo, object>> sort)
        {
            using (Speed.Begin())
            {
                dbBuilder.Desc(sort);
                DataResult.SqlBuildTime += Speed.Result.Timer.ElapsedMilliseconds;
            }
            if (sort != null) { ExpSort.Add(sort, 1); }
            return this;
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="db">事务</param>
        public List<T> ToSelectList<T>(Expression<Func<TInfo, T>> select, DbExecutor db = null)
        {
            return ToSelectList<T>(0, select, db);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="db">事务</param>
        public List<T> ToSelectList<T>(int top, Expression<Func<TInfo, T>> select, DbExecutor db = null)
        {
            return Select(select).ToList(top, db).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="db">事务</param>
        public List<T> ToSelectList<T>(int top, Expression<Func<TInfo, T?>> select, DbExecutor db = null) where T : struct
        {
            return Select(select).ToList(top, db).Select(select.Compile()).ToList().ConvertType<List<T>>();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="db">事务</param>
        public List<T> ToSelectList<T>(List<int> IDs, Expression<Func<TInfo, T>> select, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).ToSelectList<T>(select, db);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="db">事务</param>
        public List<T> ToSelectList<T>(List<int> IDs, int top, Expression<Func<TInfo, T>> select, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).ToSelectList<T>(top, select, db);
        }

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="info">已赋值的实体</param>
        /// <param name="db">可传入事务的db</param>
        public bool Insert(TInfo info, DbExecutor db = null)
        {
            var identity = DataSpeed(
                () => { return info.ID == null ? dbBuilder.Insert(info) + dbBuilder.LastIdentity() : dbBuilder.Insert(info); },
                (sql, Parms) => { return info.ID == null ? (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms) : (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(0); }
                );

            if (info.ID.GetValueOrDefault() == 0) { info.ID = identity; }
            return identity > 0;
        }

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="info">已赋值的实体</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="identity">标识，刚插入的ID</param>
        public bool Insert(TInfo info, out int identity, DbExecutor db = null)
        {
            identity = DataSpeed(
                () => { return dbBuilder.Insert(info) + dbBuilder.LastIdentity(); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(0); }
                );
            info.ID = identity;
            return identity > 0;
        }

        /// <summary>
        ///     更新数据
        /// </summary>
        /// <param name="info">已赋值的实体</param>
        /// <param name="db">可传入事务的db</param>
        public bool Update(TInfo info, DbExecutor db = null)
        {
            // 默认不带条件，并且Info.ID有值时，则自动修改当前ID
            if (ExpWhere == null && info.ID != null) { Where(o => o.ID == info.ID); }

            return DataSpeed(
                () => { return dbBuilder.Update(info); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
                (value) => { return value > 0; }
                );
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool Update(int? ID, TInfo info, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).Update(info, db);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool Update(List<int> IDs, TInfo info, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).Update(info, db);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public bool IsHaving(DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.Count(); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(0) > 0; }
                );
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool IsHaving(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).IsHaving(db);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool IsHaving(List<int> IDs, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).IsHaving(db);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public int Count(DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.Count(); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(0); }
                );
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public int Count(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).Count(db);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public int Count(List<int> IDs, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).Count(db);
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToInfo(DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToInfo(); },
                (sql, Parms) => { return (db ?? DbExtor).GetReader(CommandType.Text, sql, Parms); },
                (value) => { return value.ToInfo<TInfo>(); }
                );
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public TInfo ToInfo(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).ToInfo(db);
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToNextInfo(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID > ID).Asc(o => o.ID).ToInfo(db);
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToPreviousInfo(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID < ID).Desc(o => o.ID).ToInfo(db);
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="recordCount">返回数据总数</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        /// <param name="db">可传入事务的db</param>
        public DataTable ToTable(out int recordCount, int pageSize, int pageIndex, DbExecutor db = null)
        {
            #region 计算总页数

            recordCount = Count(db);
            var allCurrentPage = 1;

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 0)
            {
                pageSize = 0;
            }
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage)
            {
                pageIndex = allCurrentPage;
            }

            #endregion

            return DataSpeed(
                () => { return dbBuilder.ToTable(pageSize, pageIndex); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value; }
                );
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top">Top数量</param>
        /// <param name="db">可传入事务的db</param>
        public DataTable ToTable(int top = 0, DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToTable(top); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value; }
                );
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="pageIndex">分页索引，为1时，使用Top方法</param>
        /// <param name="pageSize">为0时，获取所有数据</param>
        /// <param name="db">可传入事务的db</param>
        public DataTable ToTable(int pageSize, int pageIndex, DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToTable(pageSize, pageIndex); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value; }
                );
        }

        /// <summary>
        ///     获取随机列表
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="top">Top数量</param>
        public DataTable ToDataTableByRand(int top = 0, DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToTableByRand(top); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value; }
                );
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="recordCount">返回数据总数</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(out int recordCount, int pageSize, int pageIndex, DbExecutor db = null)
        {
            #region 计算总页数

            recordCount = Count(db);
            var allCurrentPage = 1;

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 0)
            {
                pageSize = 0;
            }
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage)
            {
                pageIndex = allCurrentPage;
            }

            #endregion

            var val = DataSpeed(
                () => { return dbBuilder.ToTable(pageSize, pageIndex); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value.ToList<TInfo>(); }
                );

            return val;
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top">Top数量</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(int top = 0, DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToTable(top); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value.ToList<TInfo>(); }
                );
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="pageIndex">分页索引，为1时，使用Top方法</param>
        /// <param name="pageSize">为0时，获取所有数据</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(int pageSize, int pageIndex, DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToTable(pageSize, pageIndex); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value.ToList<TInfo>(); }
                );
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(List<int> IDs, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).ToList(0, db);
        }

        /// <summary>
        ///     通用的分页方法(多条件)
        /// </summary>
        /// <param name="rpt">Repeater带分页控件</param>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(Repeater rpt, DbExecutor db = null)
        {
            int recordCount;
            var lst = ToList(out recordCount, rpt.PageSize, rpt.PageIndex, db);
            rpt.PageCount = recordCount;

            return lst;
        }

        /// <summary>
        ///     获取所有列表
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public List<TInfo> ToList(DbExecutor db)
        {
            return ToList(0, db);
        }

        /// <summary>
        ///     获取随机列表
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="top">Top数量</param>
        public List<TInfo> ToListByRand(int top = 0, DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.ToTableByRand(top); },
                (sql, Parms) => { return (db ?? DbExtor).GetDataTable(CommandType.Text, sql, Parms); },
                (value) => { return value.ToList<TInfo>(); }
                );
        }

        /// <summary>
        ///     获取Sum总和
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        public T GetSum<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), DbExecutor db = null)
        {
            Select(select);
            return DataSpeed(
                () =>
                {
                    return dbBuilder.GetSum();
                },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(defValue); }
                );
        }

        /// <summary>
        ///     获取Sum总和
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="defValue">默认值</param>
        public T GetSum<T>(T defValue, DbExecutor db = null)
        {
            return GetSum(null, defValue, db);
        }

        /// <summary>
        ///     获取Max最大值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        public T GetMax<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), DbExecutor db = null)
        {
            Select(select);
            return DataSpeed(
                () =>
                {
                    return dbBuilder.GetMax();
                },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(defValue); }
                );
        }

        /// <summary>
        ///     获取Max最大值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="defValue">默认值</param>
        public T GetMax<T>(T defValue, DbExecutor db = null)
        {
            return GetMax(null, defValue, db);
        }

        /// <summary>
        ///     获取Min最小值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        public T GetMin<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), DbExecutor db = null)
        {
            Select(select);
            return DataSpeed(
                () =>
                {
                    return dbBuilder.GetMin();
                },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType(defValue); }
                );
        }

        /// <summary>
        ///     获取Min最小值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="defValue">默认值</param>
        public T GetMin<T>(T defValue, DbExecutor db = null)
        {
            return GetMin(null, defValue, db);
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="defValue">默认值</param>
        /// <param name="select">字段选择器</param>
        /// <param name="db">可传入事务的db</param>
        public T GetValue<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), DbExecutor db = null)
        {
            Select(select);
            return DataSpeed(
                () =>
                {
                    return dbBuilder.GetValue();
                },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteScalar(CommandType.Text, sql, Parms); },
                (value) => { return value.ConvertType<T>(); }
                );
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="defValue">默认值</param>
        /// <param name="db">可传入事务的db</param>
        public T GetValue<T>(T defValue, DbExecutor db = null)
        {
            return GetValue((Expression<Func<TInfo, T>>)null, defValue, db);
        }

        /// <summary>
        ///     获取单个字段的数据
        /// </summary>
        /// <typeparam name="T">值类型变量</typeparam>
        /// <param name="defValue">为null时默认值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="select">字段选择器</param>
        public T GetValue<T>(int? ID, Expression<Func<TInfo, T>> select, T defValue = default(T), DbExecutor db = null)
        {
            return Where(o => o.ID == ID).GetValue(select, defValue, db);
        }

        /// <summary>
        ///     获取单个字段的数据
        /// </summary>
        /// <typeparam name="T">值类型变量</typeparam>
        /// <param name="defValue">为null时默认值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public T GetValue<T>(int? ID, T defValue = default(T), DbExecutor db = null)
        {
            return GetValue(ID, null, defValue, db);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">o => o.ID == ID</param>
        public bool UpdateValue<T>(int? ID, Expression<Func<TInfo, T>> select, T fieldValue, DbExecutor db = null) where T : struct
        {
            return Where(o => o.ID == ID).UpdateValue(select, fieldValue, db);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">o => o.ID == ID</param>
        public bool UpdateValue<T>(int? ID, T fieldValue, DbExecutor db = null) where T : struct
        {
            return UpdateValue(ID, null, fieldValue, db);
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="db">可传入事务的db</param>
        public bool UpdateValue<T>(Expression<Func<TInfo, T>> select, T fieldValue, DbExecutor db = null) where T : struct
        {
            Select(select);

            return DataSpeed(
                () => { return dbBuilder.UpdateValue(fieldValue); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
                (value) => { return value > 0; }
                );
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="db">可传入事务的db</param>
        public bool UpdateValue<T>(T fieldValue, DbExecutor db = null) where T : struct
        {
            return UpdateValue((Expression<Func<TInfo, T>>)null, fieldValue, db);
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public bool Delete(DbExecutor db = null)
        {
            return DataSpeed(
                () => { return dbBuilder.Delete(); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
                (value) => { return value > 0; }
                );
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="db">可传入事务的db</param>
        public bool Delete(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).Delete(db);
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="db">可传入事务的db</param>
        public bool Delete(List<int> IDs, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).Delete(db);
        }

        /// <summary>
        ///     重置标识
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public void ResetIdentity(DbExecutor db = null)
        {
            DataSpeed(
                () => { return dbBuilder.ResetIdentity(); },
                (sql, Parms) => { return (db ?? DbExtor).ExecuteNonQuery(CommandType.Text, sql, Parms); },
                (value) => { return value; }
                );
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="actInfo">对新职的赋值</param>
        /// <param name="db">可传入事务的db</param>
        public void Copy(Action<TInfo> actInfo = null, DbExecutor db = null)
        {
            var lst = ToList(db);
            foreach (var info in lst)
            {
                info.ID = null;
                if (actInfo != null) actInfo(info);
                Insert(info, db);
            }
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="actInfo">对新职的赋值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">o => o.ID == ID</param>
        public void Copy(int? ID, Action<TInfo> actInfo = null, DbExecutor db = null)
        {
            Where(o => o.ID == ID);
            Copy(actInfo, db);
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="actInfo">对新职的赋值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        public void Copy(List<int> IDs, Action<TInfo> actInfo = null, DbExecutor db = null)
        {
            Where(o => IDs.Contains(o.ID));
            Copy(actInfo, db);
        }

        /// <summary>
        /// dbBuilder.ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return dbBuilder.ToString();
        }

        /// <summary>
        /// 指量插入数据（仅支付Sql Server)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据</param>
        public void SqlBulkCopy(List<TInfo> lst, DbExecutor db = null)
        {
            var dt = lst.ToTable();
            DataSpeed(
                () => { return ""; },
                (sql, Parms) => { (db ?? DbExtor).ExecuteSqlBulkCopy(TableName, dt); return true; },
                (value) => { return value; }
                );
        }

        /// <summary>
        /// 指量插入数据（仅支付Sql Server)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据</param>
        public void SqlBulkCopy(DataTable dt, DbExecutor db = null)
        {
            DataSpeed(
                () => { return ""; },
                (sql, Parms) => { (db ?? DbExtor).ExecuteSqlBulkCopy(TableName, dt); return true; },
                (value) => { return value; }
                );
        }

        /// <summary>
        ///     计算执行使用的时间
        /// </summary>
        /// <typeparam name="T">返回值</typeparam>
        /// <typeparam name="T2">数据库中返回的值</typeparam>
        /// <param name="actBuilderSql">生成Sql步骤</param>
        /// <param name="actGetData">获取数据步骤</param>
        /// <param name="actParseData">转换数据类型步骤</param>
        private T DataSpeed<T, T2>(Func<string> actBuilderSql, Func<string, DbParameter[], T2> actGetData, Func<T2, T> actParseData)
        {
            using (Speed.Begin())
            {
                //dbBuilder.Clear();
                //dbBuilder.Where(ExpWhere);  // 生成条件SQL
                //foreach (var item in ExpSelect) { dbBuilder.Select(item); }  // 生成筛选SQL
                //foreach (var item in ExpSort) { if (item.Value == 0) { dbBuilder.Asc(item.Key); } else { dbBuilder.Desc(item.Key); } }  // 生成排序SQL

                DataResult.Sql = actBuilderSql();
                DataResult.ParmsList = dbBuilder.ParamsList;
                DataResult.SqlBuildTime += Speed.Result.Timer.Elapsed.TotalMilliseconds;

                // 获取数据
                Speed.Result.Timer.Restart();
                var value = actGetData(DataResult.Sql, DataResult.ParmsList.ToArray());
                DataResult.OperateDataTime += Speed.Result.Timer.Elapsed.TotalMilliseconds;

                // 转换数据
                Speed.Result.Timer.Restart();
                var v = actParseData(value);
                DataResult.ToModelTime += Speed.Result.Timer.Elapsed.TotalMilliseconds;

                // 记录执行过程
                SqlTemplate.Output(DataResult);
                return v;
            }
        }
    }

    /// <summary>
    /// 片断数据缓存基类
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    public class CacheDataBean<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     数据库持久化
        /// </summary>
        private readonly Bean<TInfo> Data;

        /// <summary>
        ///     兼容Qyn.Factory项目
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="commandTimeout">命令执时超时时间</param>
        /// <param name="tableName">表名称</param>
        public CacheDataBean(DataBaseType dbType, string connetionString, int commandTimeout, string tableName = "")
        {
            Data = new Bean<TInfo>(dbType, connetionString, commandTimeout, tableName);
        }

        /// <summary>
        ///     默认使用
        /// </summary>
        /// <param name="tableName">表名称</param>
        public CacheDataBean(string tableName = "")
        {
            Data = new Bean<TInfo>(tableName);
        }

        /// <summary>
        ///     返回执行结果
        /// </summary>
        public DataResult<TInfo> DataResult
        {
            get { return Data.DataResult; }
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public CacheDataBean<TInfo> Where(Expression<Func<TInfo, bool>> where)
        {
            Data.Where(where);
            return this;
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        /// <param name="where">查询条件</param>
        public CacheDataBean<TInfo> WhereOr(Expression<Func<TInfo, bool>> where)
        {
            Data.WhereOr(where);
            return this;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public CacheDataBean<TInfo> Select<T>(Expression<Func<TInfo, T>> select)
        {
            Data.Select(select);
            return this;
        }

        /// <summary>
        ///     升序
        /// </summary>
        /// <param name="sort">升序</param>
        public CacheDataBean<TInfo> Asc(Expression<Func<TInfo, object>> sort)
        {
            Data.Asc(sort);
            return this;
        }

        /// <summary>
        ///     降序
        /// </summary>
        /// <param name="sort">降序</param>
        public CacheDataBean<TInfo> Desc(Expression<Func<TInfo, object>> sort)
        {
            Data.Desc(sort);
            return this;
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="db">可传入事务的db</param>
        public List<T> ToSelectList<T>(Expression<Func<TInfo, T>> select, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return ToSelectList(select, 0, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="db">可传入事务的db</param>
        public List<T> ToSelectList<T>(Expression<Func<TInfo, T>> select, int top = 0, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Select(select).ToList(top, isIgnoreStatistics, db).Select(select.Compile()).ToList();
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="db">可传入事务的db</param>
        public List<T> ToSelectList<T>(List<int> IDs, Expression<Func<TInfo, T>> select, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).ToSelectList<T>(select, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     返回筛选后的列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="IDs">o => IDs.Contains(o.ID)</param>
        /// <param name="db">可传入事务的db</param>
        public List<T> ToSelectList<T>(List<int> IDs, Expression<Func<TInfo, T>> select, int top = 0, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).ToSelectList<T>(select, top, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public int Count(bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.Count, () => Data.Count(db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public int Count(int? ID, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).Count(isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public int Count(List<int> IDs, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).Count(isIgnoreStatistics, db);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public bool IsHaving(bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.IsHaving, () => Data.Count(db) > 0, isIgnoreStatistics);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool IsHaving(int? ID, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).IsHaving(isIgnoreStatistics, db);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool IsHaving(List<int> IDs, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).IsHaving(isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public TInfo ToInfo(bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.Info, () => Data.ToInfo(db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="recordCount">返回数据总数</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public DataTable ToTable(out int recordCount, int pageSize, int pageIndex, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            recordCount = Count(isIgnoreStatistics, db);
            return DataCache.Get(Data.ToString() + "；pageSize：" + pageSize + "；pageIndex：" + pageIndex, DataCache.enumDataType.Table, () => Data.ToTable(pageSize, pageIndex, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top">Top数量</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public DataTable ToTable(int top = 0, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString() + "；Top：" + top, DataCache.enumDataType.Table, () => Data.ToTable(top, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="pageIndex">分页索引，为1时，使用Top方法</param>
        /// <param name="pageSize">为0时，获取所有数据</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public DataTable ToTable(int pageSize, int pageIndex, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString() + "；pageSize：" + pageSize + "；pageIndex：" + pageIndex, DataCache.enumDataType.Table, () => Data.ToTable(pageSize, pageIndex, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="recordCount">返回数据总数</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public List<TInfo> ToList(out int recordCount, int pageSize, int pageIndex, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            recordCount = Count(isIgnoreStatistics, db);
            return DataCache.Get(Data.ToString() + "；pageSize：" + pageSize + "；pageIndex：" + pageIndex, DataCache.enumDataType.List, () => Data.ToList(pageSize, pageIndex, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top">TOP数量</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public List<TInfo> ToList(int top = 0, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString() + "；Top：" + top, DataCache.enumDataType.IsHaving, () => Data.ToList(top, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="pageIndex">分页索引，为1时，使用Top方法</param>
        /// <param name="pageSize">为0时，获取所有数据</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public List<TInfo> ToList(int pageSize, int pageIndex, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString() + "；pageSize：" + pageSize + "；pageIndex：" + pageIndex, DataCache.enumDataType.List, () => Data.ToList(pageSize, pageIndex, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取分页、Top、全部的数据方法(根据pageSize、pageIndex自动识别使用场景)
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public List<TInfo> ToList(List<int> IDs, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).ToList(0, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     通用的分页方法(多条件)
        /// </summary>
        /// <param name="rpt">Repeater带分页控件</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public List<TInfo> ToList(Repeater rpt, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            int recordCount;
            var lst = ToList(out recordCount, rpt.PageSize, rpt.PageIndex, isIgnoreStatistics, db);
            rpt.PageCount = recordCount;

            return lst;
        }

        /// <summary>
        ///     获取所有列表
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public List<TInfo> ToList(bool isIgnoreStatistics, DbExecutor db)
        {
            return ToList(0, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取Sum总和
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetSum<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.Sum, () => Data.GetSum(select, defValue, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取Sum总和
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetSum<T>(T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return GetSum(null, defValue, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取Max最大值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetMax<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.Max, () => Data.GetMax(select, defValue, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取Max最大值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetMax<T>(T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return GetMax(null, defValue, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取Min最小值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetMin<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.Min, () => Data.GetMin(select, defValue, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取Min最小值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="db">可传入事务的db</param>
        /// <param name="defValue">默认值</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetMin<T>(T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return GetMin(null, defValue, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="defValue">默认值</param>
        /// <param name="select">字段选择器</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetValue<T>(Expression<Func<TInfo, T>> select, T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return DataCache.Get(Data.ToString(), DataCache.enumDataType.Value, () => Data.GetValue(select, defValue, db), isIgnoreStatistics);
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="defValue">默认值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetValue<T>(T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return GetValue((Expression<Func<TInfo, T>>)null, defValue, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取单个字段的数据
        /// </summary>
        /// <typeparam name="T">值类型变量</typeparam>
        /// <param name="defValue">为null时默认值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="select">字段选择器</param>
        public T GetValue<T>(int? ID, Expression<Func<TInfo, T>> select, T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).GetValue(select, defValue, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取单个字段的数据
        /// </summary>
        /// <typeparam name="T">值类型变量</typeparam>
        /// <param name="defValue">为null时默认值</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public T GetValue<T>(int? ID, T defValue = default(T), bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return GetValue(ID, null, defValue, isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToNextInfo(int? ID, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => o.ID > ID).Asc(o => o.ID).ToInfo(isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        /// <param name="db">可传入事务的db</param>
        public TInfo ToPreviousInfo(int? ID, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => o.ID < ID).Desc(o => o.ID).ToInfo(isIgnoreStatistics, db);
        }

        /// <summary>
        ///     获取单条记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        public TInfo ToInfo(int? ID, bool isIgnoreStatistics = false, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).ToInfo(isIgnoreStatistics, db);
        }

        /// <summary>
        /// Data.ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Data.ToString();
        }
    }
}