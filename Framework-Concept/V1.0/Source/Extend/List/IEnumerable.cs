using FS.Core.Model;
using FS.UI;
using FS.Utils.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Extend
{
    /// <summary>
    ///     Data扩展工具
    /// </summary>
    public static partial class ListExtend
    {
        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst">要拼接的LIST</param>
        /// <param name="sign">分隔符</param>
        public static string ToString(this IEnumerable lst, string sign = ",")
        {
            if (lst == null) { return string.Empty; }
            var str = new StringBuilder();
            foreach (var item in lst) { str.Append(item + sign); }
            return str.ToString().DelEndOf(sign);
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TInfo ToNextInfo<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => o.ID > ID).OrderBy(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TInfo ToPreviousInfo<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => o.ID < ID).OrderByDescending(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, int pageSize, int pageIndex = 1)
        {
            if (pageSize == 0) { return lst.ToList(); }

            #region 计算总页数

            var allCurrentPage = 0;
            var recordCount = lst.Count();
            if (pageIndex < 1) { pageIndex = 1; return lst.Take(pageSize).ToList(); }
            if (pageSize < 1) { pageSize = 10; }

            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            if (pageIndex > allCurrentPage) { pageIndex = allCurrentPage; }

            #endregion

            return lst.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs, int pageSize, int pageIndex = 1) where TInfo : BaseInfo<int>, new()
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID)), pageSize, pageIndex);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="rpt">Repeater</param>
        /// <returns></returns>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, Repeater rpt)
        {
            rpt.PageCount = lst.Count();
            return ToList(lst, rpt.PageSize, rpt.PageIndex);
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="rpt">Repeater</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs, Repeater rpt) where TInfo : BaseInfo<int>, new()
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID)), rpt);
        }

        /// <summary>
        ///     获取List列表
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).ToList();
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        public static TInfo ToInfo<TInfo>(this IEnumerable<TInfo> lst)
        {
            return lst.FirstOrDefault();
        }

        /// <summary>
        ///     获取Info
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID的操作</param>
        public static TInfo ToInfo<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : BaseInfo<int>, new()
        {
            return lst.FirstOrDefault(o => o.ID == ID);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static int Count<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).Count();
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        public static int Count<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => o.ID == ID).Count();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        public static bool IsHaving<TInfo>(this IEnumerable<TInfo> lst)
        {
            return lst.Count() > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool IsHaving<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => o.ID == ID).Count() > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool IsHaving<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).Count() > 0;
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">BaseModel</typeparam>
        public static T GetValue<TInfo, T>(this IEnumerable<TInfo> lst, Func<TInfo, T> select, T defValue = default(T))
        {
            if (lst == null) { return defValue; }
            var value = lst.Select(select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">BaseModel</typeparam>
        public static T GetValue<TInfo, T>(this IEnumerable<TInfo> lst, int ID, Func<TInfo, T> select, T defValue = default(T)) where TInfo : BaseInfo<int>, new()
        {
            if (lst == null) { return defValue; }
            lst = lst.Where(o => o.ID == ID).ToList();
            if (lst == null || lst.Count() == 0)
            {
                return defValue;
            }

            var value = lst.Select(select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TInfo, T>(this IEnumerable<TInfo> lst, Func<TInfo, T> select)
        {
            return lst.Select(select).ToList();
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TInfo, T>(this IEnumerable<TInfo> lst, int top, Func<TInfo, T> select)
        {
            return lst.Select(select).Take(top).ToList();
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TInfo, T>(this IEnumerable<TInfo> lst, List<int> IDs, Func<TInfo, T> select) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).ToSelectList(select);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TInfo, T>(this IEnumerable<TInfo> lst, List<int> IDs, int top, Func<TInfo, T> select) where TInfo : BaseInfo<int>, new()
        {
            return lst.ToSelectList(IDs, select).Take(top).ToList();
        }

        /// <summary>
        ///     不重复列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<TInfo> ToDistinctList<TInfo, T>(this IEnumerable<TInfo> lst, Func<TInfo, T> select) where TInfo : BaseInfo<int>, new()
        {
            return lst.Distinct(new InfoComparer<TInfo, T>(select)).ToList();
        }

        /// <summary>
        ///     不重复列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<TInfo> ToDistinctList<TInfo, T>(this IEnumerable<TInfo> lst, List<int> IDs, Func<TInfo, T> select) where TInfo : BaseInfo<int>, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).Distinct(new InfoComparer<TInfo, T>(select)).ToList();
        }
    }
}