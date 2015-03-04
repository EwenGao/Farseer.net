using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using FS.Configs;
using FS.Core.Bean;
using FS.Core.Data;
using FS.Extend;
using FS.Mapping.Table;
using FS.Utils.Common;
using FS.Utils.Web;

namespace FS.Core.Model
{
    /// <summary>
    ///     实体类基类信息
    /// </summary>
    public class BaseCacheModel<TInfo> : ModelInfo where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     查询结果
        /// </summary>
        public static DataResult<TInfo> DataResult { get; set; }

        /// <summary>
        ///     数据库持久化
        /// </summary>
        public static CacheBean<TInfo> Data
        {
            get
            {
                var bean = new CacheBean<TInfo>();
                DataResult = bean.DataResult;
                return bean;
            }
        }

        /// <summary>
        ///     数据缓存操作
        /// </summary>
        /// <param name="db">事务</param>
        public static List<TInfo> Cache(DbExecutor db = null)
        {
            return Data.ToList(db);
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
    /// CacheBean
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    public class CacheBean<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     数据库持久化
        /// </summary>
        private readonly Bean<TInfo> Data;

        /// <summary>
        ///     实体映射
        /// </summary>
        private readonly TableMap Map = typeof(TInfo);

        /// <summary>
        ///     Select表达式
        /// </summary>
        private Expression<Func<TInfo, object>> ExpSelect;

        /// <summary>
        ///     Where表达式
        /// </summary>
        private Expression<Func<TInfo, bool>> ExpWhere;
        /// <summary>
        /// 表名
        /// </summary>
        private string TableName;

        /// <summary>
        ///     兼容Qyn.Factory项目
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        /// <param name="commandTimeout">命令执时超时时间</param>
        /// <param name="tableName">表名称</param>
        public CacheBean(DataBaseType dbType, string connetionString, int commandTimeout, string tableName = "")
        {
            TableName = tableName.IsNullOrEmpty() ? Map.ClassInfo.Name : tableName;
            Key = Encrypt.MD5(connetionString + Map.Type.FullName);
            Data = new Bean<TInfo>(dbType, connetionString, commandTimeout, TableName);
        }

        /// <summary>
        ///     默认使用
        /// </summary>
        /// <param name="tableName">表名称</param>
        public CacheBean(string tableName = "")
        {
            TableName = tableName.IsNullOrEmpty() ? Map.ClassInfo.Name : tableName;
            Key = Encrypt.MD5(Map.ClassInfo.ConnStr + Map.Type.FullName);
            var dbType = Map.ClassInfo.DataType;
            var connetionString = Map.ClassInfo.ConnStr;
            var commandTimeout = Map.ClassInfo.CommandTimeout;

            Data = new Bean<TInfo>(dbType, connetionString, commandTimeout, TableName);
        }

        /// <summary>
        ///     Cache Key
        /// </summary>
        private string Key { get; set; }

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
        public CacheBean<TInfo> Where(Expression<Func<TInfo, bool>> where)
        {
            ExpWhere = ExpWhere.AndAlso(where);
            return this;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        public CacheBean<TInfo> Select(Expression<Func<TInfo, object>> select)
        {
            ExpSelect = select;
            return this;
        }

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="info">已赋值的实体</param>
        /// <param name="db">可传入事务的db</param>
        public bool Insert(TInfo info, DbExecutor db = null)
        {
            int identity;
            return Insert(info, out identity, db);
        }

        /// <summary>
        ///     插入记录
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="identity">标识</param>
        public bool Insert(TInfo info, out int identity, DbExecutor db = null)
        {
            bool result;
            var indexHaveValue = Map.GetModelInfo().Key != null
                                      ? Map.GetModelInfo().Key.GetValue(info, null) != null
                                      : false;
            var lst = ToList(db);

            // 如果标识没有值，则必须取值。
            if (indexHaveValue)
            {
                result = Data.Insert(info, db);
                identity = Map.GetModelInfo().Key.GetValue(info, null).ConvertType(0);
            }
            else
            {
                result = Data.Insert(info, out identity, db);

                #region 赋值给主键

                if (!Map.IndexName.IsNullOrEmpty())
                {
                    var kic = Map.GetModelInfo(Map.IndexName);
                    if (kic.Key.CanWrite)
                    {
                        kic.Key.SetValue(info, identity, null);
                    }
                }

                #endregion
            }

            if (result)
            {
                lst.Add(info);
            }
            return result;
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>bu
        public bool Update(TInfo info, DbExecutor db = null)
        {
            // 默认不带条件，并且Info.ID有值时，则自动修改当前ID
            if (ExpWhere == null && info.ID != null) { Where(o => o.ID == info.ID); }
            var result = Data.Where(ExpWhere).Update(info, db);

            if (result)
            {
                var lst = ToList(db);
                if (ExpWhere != null) { lst = lst.FindAll(ExpWhere.Compile().ToPredicate()); }

                foreach (var item in lst)
                {
                    foreach (var kic in Map.ModelList.Where(o => o.Value.IsDbField))
                    {
                        var objValue = kic.Key.GetValue(info, null);
                        if (objValue == null || !kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(item, objValue, null);
                    }
                }
            }
            return result;
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
        ///     更新数据(在原字段值上+ -值)
        /// </summary>
        /// <param name="fieldValue">字段值(请先指定select字段)</param>
        /// <param name="db">可传入事务的db</param>
        public bool UpdateValue<T>(T fieldValue, DbExecutor db = null) where T : struct
        {
            var result = Data.Where(ExpWhere).Select(ExpSelect).UpdateValue(fieldValue, db);

            if (result)
            {
                //获取索引的属性
                var kic = Map.GetModelInfo(ExpSelect.GetUsedName());

                var lst = ToList(db);
                if (ExpWhere != null)
                {
                    lst = lst.FindAll(ExpWhere.Compile().ToPredicate());
                }

                var type = typeof(T);
                // 判断是否为泛型
                if (type.IsGenericType)
                {
                    // 非 Nullable<> 类型
                    if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                    {
                        type = type.GetGenericArguments()[0];
                    }
                    else
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }
                }

                foreach (var info in lst)
                {
                    var value = kic.Key.GetValue(info, null).ConvertType(default(T));
                    if (!kic.Key.CanWrite)
                    {
                        continue;
                    }

                    object oVal;
                    switch (type.Name)
                    {
                        case "Int32":
                        case "Int16":
                        case "Byte":
                            oVal = value.ConvertType(0) + fieldValue.ConvertType(0);
                            break;
                        case "Decimal":
                        case "Long":
                        case "Float":
                        case "Double":
                            oVal = value.ConvertType(0m) + fieldValue.ConvertType(0m);
                            break;
                        default:
                            throw new Exception("类型：" + type.Name + "， 未有转换程序对其解析。");
                    }

                    kic.Key.SetValue(info, oVal, null);
                }
            }
            return result;
        }

        /// <summary>
        ///     更新数据(在原字段值上+ -值)
        /// </summary>
        /// <param name="fieldValue">字段值(请先指定select字段)</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <typeparam name="T">默认值类型</typeparam>
        public bool UpdateValue<T>(int? ID, T fieldValue, DbExecutor db = null) where T : struct
        {
            return Where(o => o.ID == ID).UpdateValue(fieldValue, db);
        }

        /// <summary>
        ///     更新数据(在原字段值上+ -值)
        /// </summary>
        /// <param name="fieldValue">字段值(请先指定select字段)</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <typeparam name="T">默认值类型</typeparam>
        public bool UpdateValue<T>(int? ID, T? fieldValue, DbExecutor db = null) where T : struct
        {
            return UpdateValue(ID, fieldValue.GetValueOrDefault());
        }

        /// <summary>
        ///     更新数据(在原字段值上+ -值)
        /// </summary>
        /// <param name="fieldValue">字段值(请先指定select字段)</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">默认值类型</typeparam>
        public bool UpdateValue<T>(List<int> IDs, T fieldValue, DbExecutor db = null) where T : struct
        {
            return Where(o => IDs.Contains(o.ID)).UpdateValue(fieldValue, db);
        }

        /// <summary>
        ///     更新数据(在原字段值上+ -值)
        /// </summary>
        /// <param name="fieldValue">字段值(请先指定select字段)</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">默认值类型</typeparam>
        public bool UpdateValue<T>(List<int> IDs, T? fieldValue, DbExecutor db = null) where T : struct
        {
            return UpdateValue(IDs, fieldValue.GetValueOrDefault());
        }

        /// <summary>
        ///     删除记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public bool Delete(DbExecutor db = null)
        {
            var result = Data.Where(ExpWhere).Delete(db);

            if (result)
            {
                if (ExpWhere == null)
                {
                    GetTrueList(db);
                }
                else
                {
                    var lst = ToList(db).RemoveAll(ExpWhere.Compile().ToPredicate());
                }
            }
            return result;
        }

        /// <summary>
        ///     删除记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public bool Delete(int? ID, DbExecutor db = null)
        {
            return Where(o => o.ID == ID).Delete(db);
        }

        /// <summary>
        ///     删除记录
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public bool Delete(List<int> IDs, DbExecutor db = null)
        {
            return Where(o => IDs.Contains(o.ID)).Delete(db);
        }

        /// <summary>
        ///     根据ID判断数据是否存在来调用(Insert 还是 Update)
        /// </summary>
        /// <param name="info">实体类</param>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public int Save(int ID, TInfo info, DbExecutor db = null)
        {
            if (ID > 0)
            {
                ID = Where(o => o.ID == ID).Update(info, db) ? ID : 0;
                if (ID > 0)
                {
                    return ID;
                }
            }
            Insert(info, out ID, db);
            return ID;
        }

        /// <summary>
        ///     获取缓存中的列表
        /// </summary>
        /// <param name="db">事务</param>
        internal List<TInfo> ToList(DbExecutor db = null)
        {
            var lst = WebCache.Get<List<TInfo>>(Key);
            if (lst == null)
            {
                lst = GetTrueList(db) ?? new List<TInfo>();
            }
            if (lst.Count == 0)
            {
                WebCache.Add(Key, lst, SystemConfigs.ConfigInfo.Cache_Db_TimeOut);
            }
            return lst;
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="actInfo">对新职的赋值</param>
        /// <param name="db">可传入事务的db</param>
        public void Copy(Action<TInfo> actInfo = null, DbExecutor db = null)
        {
            var lst = ToList(db);
            if (ExpWhere != null)
            {
                lst = lst.FindAll(ExpWhere.Compile().ToPredicate()).Clone();
            }

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
        ///     获取数据列表(读取数据库)
        /// </summary>
        /// <param name="db">事务</param>
        public List<TInfo> GetTrueList(DbExecutor db = null)
        {
            var lst = Data.ToList(db);
            WebCache.Add(Key, lst, SystemConfigs.ConfigInfo.Cache_Db_TimeOut);
            return lst;
        }
    }
}