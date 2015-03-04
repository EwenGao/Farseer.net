using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using FS.Core.Bean;
using FS.Core.Data;
using FS.Extend;
using FS.Utils.Web;

namespace FS.Core.Model
{
    /// <summary>
    ///     实体类基类信息
    /// </summary>
    public class BaseCateModel<TInfo> : ModelCateInfo where TInfo : ModelCateInfo, new()
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

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public virtual bool Insert(DbExecutor db = null)
        {
            return Data.Insert(Clone<TInfo>(), db);
        }

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="identity">标识，刚插入的ID</param>
        public virtual bool Insert(out int identity, DbExecutor db = null)
        {
            return Data.Insert(Clone<TInfo>(), out identity, db);
        }

        /// <summary>
        ///     更新数据
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="db">可传入事务的db</param>
        public virtual bool Update(Expression<Func<TInfo, bool>> where, DbExecutor db = null)
        {
            return Data.Where(where).Update(Clone<TInfo>(), db);
        }

        /// <summary>
        ///     更新数据
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="db">可传入事务的db</param>
        public virtual bool Update(DbExecutor db = null)
        {
            return Data.Update(Clone<TInfo>(), db);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public virtual bool Update(int? ID, DbExecutor db = null)
        {
            return Data.Where(o => o.ID == ID).Update(Clone<TInfo>(), db);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public virtual bool Update(List<int> IDs, DbExecutor db = null)
        {
            return Data.Where(o => IDs.Contains(o.ID)).Update(Clone<TInfo>(), db);
        }

        /// <summary>
        ///     保存数据
        /// </summary>
        /// <param name="reqID">请求ID</param>
        /// <param name="tip">提示</param>
        /// <param name="actInsert">插入时的方法委托</param>
        /// <param name="actUpdate">更新时的方法委托</param>
        /// <param name="actSuccess">成功后的方法委托</param>
        public virtual void Save(int reqID, Action<string, string> tip = null, Action<TInfo, DbExecutor> actInsert = null,
                         Action<TInfo, DbExecutor> actUpdate = null, Action<int, TInfo, DbExecutor> actSuccess = null)
        {
            var info = Clone<TInfo>();
            if (!info.Check(tip))
            {
                return;
            }

            using (DbExecutor db = typeof(TInfo))
            {
                if (reqID > 0)
                {
                    if (actUpdate != null)
                    {
                        actUpdate(info, db);
                    }
                    Update(reqID, db);
                }
                else
                {
                    if (actInsert != null)
                    {
                        actInsert(info, db);
                    }
                    Insert(out reqID, db);
                }
                if (actSuccess != null)
                {
                    actSuccess(reqID, info, db);
                }
                db.Commit();
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
}