using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using FS.Core.Data;
using FS.Core.Model;
using FS.Mapping.Table;
using FS.Utils.Common;
using Repeater = FS.UI.Repeater;

namespace FS.Extend
{
    /// <summary>
    ///     Data扩展工具
    /// </summary>
    public static class ListExtend
    {
        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerator<int> lst, int? value)
        {
            return lst.Contains(value.GetValueOrDefault());
        }

        /// <summary>
        ///     数据分页
        /// </summary>
        /// <typeparam name="TSource">实体</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        public static IQueryable<TSource> Split<TSource>(this IQueryable<TSource> source, int pageSize = 20, int pageIndex = 1)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 20;
            }
            return source.Skip(pageSize*(pageIndex - 1)).Take(pageSize);
        }
        /// <summary>
        ///     关联两个实体
        /// </summary>
        /// <typeparam name="T1">主实体</typeparam>
        /// <typeparam name="T2">要附加关联的实体</typeparam>
        /// <param name="lst">主列表</param>
        /// <param name="JoinModule">要关联的子实体</param>
        /// <param name="JoinModuleSelect">要附加关联的子实体的字段筛选</param>
        /// <param name="JoinModuleID">主表关系字段</param>
        /// <param name="defJoinModule">为空时如何处理？</param>
        /// <param name="db">事务</param>
        public static List<T1> Join<T1, T2>(this List<T1> lst, Expression<Func<T1, T2>> JoinModule,
                                            Func<T1, int?> JoinModuleID = null,
                                            Expression<Func<T2, object>> JoinModuleSelect = null,
                                            T2 defJoinModule = null, DbExecutor db = null)
            where T1 : ModelInfo, new()
            where T2 : ModelInfo, new()
        {
            if (lst == null || lst.Count == 0) { return lst; }
            if (JoinModuleID == null) { JoinModuleID = o => o.ID; }

            #region 获取实际类型

            var memberExpression = JoinModule.Body as MemberExpression;
            // 获取属性类型
            var propertyType = (PropertyInfo)memberExpression.Member;

            var lstPropery = new List<PropertyInfo>();
            while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = memberExpression.Expression as MemberExpression;
                lstPropery.Add((PropertyInfo)memberExpression.Member);
            }
            lstPropery.Reverse();

            #endregion

            // 内容ID
            var lstIDs = lst.Select(JoinModuleID).ToList().Select(o => o.GetValueOrDefault()).ToList();
            // 详细资料
            var lstSub = (new T2()) is BaseCacheModel<T2>
                                  ? BaseCacheModel<T2>.Cache(db).ToList(lstIDs)
                                  : BaseModel<T2>.Data.Where(o => lstIDs.Contains(o.ID))
                                                 .Select(JoinModuleSelect)
                                                 .Select(o => o.ID)
                                                 .ToList(db);

            foreach (var item in lst)
            {
                var subInfo = lstSub.FirstOrDefault(o => o.ID == JoinModuleID.Invoke(item)) ?? defJoinModule;

                object value = item;
                foreach (var propery in lstPropery)
                {
                    value = propery.GetValue(value, null);
                }
                propertyType.SetValue(value, subInfo, null);
            }

            return lst;
        }

        /// <summary>
        ///     复制一个新的List
        /// </summary>
        public static List<T> Copy<T>(this List<T> lst)
        {
            var lstNew = new List<T>();

            if (lst != null)
            {
                lst.ForEach(o => lstNew.Add(o));
            }

            return lstNew;
        }

        /// <summary>
        ///     克隆List
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> list) where T : ICloneable
        {
            return list == null ? null : list.Select(o => (T)o.Clone()).ToList();
        }

        /// <summary>
        ///     获取List最后一项
        /// </summary>
        /// <typeparam name="T">任何对象</typeparam>
        /// <param name="lst">List列表</param>
        public static T GetLast<T>(this List<T> lst)
        {
            return lst.Count > 0 ? lst[lst.Count - 1] : default(T);
        }


        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this List<int> lst, int? value)
        {
            return lst.Contains(value.GetValueOrDefault());
        }

        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this List<uint> lst, uint? value)
        {
            return lst.Contains(value.GetValueOrDefault());
        }

        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <typeparam name="TInfo">实体</typeparam>
        /// <param name="lst">列表</param>
        /// <param name="count">生成的数据</param>
        public static List<TInfo> TestData<TInfo>(this List<TInfo> lst, int count) where TInfo : new()
        {
            lst = new List<TInfo>();
            for (var i = 0; i < count; i++)
            {
                lst.Add(new TInfo().TestData());
            }
            return lst.ToList();
        }

        /// <summary>
        /// 清除重复的词语（每项中的每个字符对比）
        /// 然后向右横移一位，按最长到最短截取匹配每一项
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<string> ClearRepeat(this List<string> lst)
        {
            for (var index = 0; index < lst.Count; index++) // 迭代所有关键词
            {
                var key = lst[index];
                for (var moveIndex = 0; moveIndex < key.Length; moveIndex += 1)     // 每次移动后2位当前关键词
                {
                    for (var step = key.Length; (step - moveIndex) >= 2; step--)   // 每次减少1位来对比
                    {
                        var clearKey = key.Substring(moveIndex, step - moveIndex);  // 截取的关键词

                        for (var index2 = index + 1; index2 < lst.Count; index2++)  // 清除下一项的所有字符串
                        {
                            lst[index2] = lst[index2].Replace(clearKey, "").Trim();
                        }
                    }
                }
            }

            lst.RemoveAll(o => o.IsNullOrEmpty());
            return lst;
        }



        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="lst">集合</param>
        /// <returns></returns>
        public static DataTable ToTable<TInfo>(this List<TInfo> lst) where TInfo : ModelInfo, new()
        {
            var dt = new DataTable();
            if (lst.Count == 0) { return dt; }
            var map = TableMapCache.GetMap(lst[0].GetType());
            var lstFields = map.ModelList.Where(o => o.Value.IsDbField);
            foreach (var field in lstFields)
            {
                var type = field.Key.PropertyType;
                // 对   List 类型处理
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
                dt.Columns.Add(field.Value.Column.Name, type);
            }

            foreach (var info in lst)
            {
                dt.Rows.Add(dt.NewRow());
                foreach (var field in lstFields)
                {
                    var value = info.GetValue(field.Key.Name, (object)null);
                    if (value == null) { continue; }
                    if (!dt.Columns.Contains(field.Value.Column.Name)) { dt.Columns.Add(field.Value.Column.Name); }
                    dt.Rows[dt.Rows.Count - 1][field.Value.Column.Name] = value;
                }
            }
            return dt;
        }

        /// <summary>
        ///     自动填充到指定数量
        /// </summary>
        public static IList Fill(this IList lst, int maxCount, object defValue)
        {
            while (true)
            {
                if (lst.Count >= maxCount)
                {
                    break;
                }
                lst.Add(defValue);
            }

            return lst;
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToTable(this IList list)
        {
            var result = new DataTable();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                foreach (var info in list)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        var obj = pi.GetValue(info, null);
                        tempList.Add(obj);
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        ///     将泛型集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToTable(this IList list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            var result = new DataTable();
            if (list.Count <= 0) { return result; }
            var propertys = list[0].GetType().GetProperties();
            foreach (var pi in propertys)
            {
                if (propertyNameList.Count == 0)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                else
                {
                    if (propertyNameList.Contains(pi.Name))
                        result.Columns.Add(pi.Name, pi.PropertyType);
                }
            }

            foreach (var info in list)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        var obj = pi.GetValue(info, null);
                        tempList.Add(obj);
                    }
                    else
                    {
                        if (!propertyNameList.Contains(pi.Name)) continue;
                        var obj = pi.GetValue(info, null);
                        tempList.Add(obj);
                    }
                }
                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
            return result;
        }
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
        public static TInfo ToNextInfo<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : ModelInfo, new()
        {
            return lst.Where(o => o.ID > ID).OrderBy(o => o.ID).FirstOrDefault();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="lst">要获取值的列表</param>
        /// <param name="ID">当前ID</param>
        public static TInfo ToPreviousInfo<TInfo>(this IEnumerable<TInfo> lst, int ID) where TInfo : ModelInfo, new()
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

        ///// <summary>
        ///// List转换成新的List
        ///// </summary>
        ///// <typeparam name="T1">源类型</typeparam>
        ///// <typeparam name="T2">新的类型</typeparam>
        ///// <param name="lst">源列表</param>
        ///// <param name="defValue">默认值</param>
        //public static List<T2> ToList<T1, T2>(this IEnumerable<T1> lst, T2 defValue) where T1 : struct
        //{
        //    List<T2> lstConvert = new List<T2>();
        //    foreach (var item in lst)
        //    {
        //        lstConvert.Add(item.ConvertType(defValue));
        //    }
        //    return lstConvert;
        //}

        ///// <summary>
        ///// List转换成新的List
        ///// </summary>
        ///// <typeparam name="T1">源类型</typeparam>
        ///// <typeparam name="T2">新的类型</typeparam>
        ///// <param name="lst">源列表</param>
        ///// <param name="func">转换方式</param>
        ///// <returns></returns>
        //public static List<T2> ToList<T1, T2>(this IEnumerable<T1> lst, Func<T1,T2> func) where T1 : struct
        //{
        //    List<T2> lstConvert = new List<T2>();
        //    foreach (var item in lst)
        //    {
        //        lstConvert.Add(func(item));
        //    }
        //    return lstConvert;
        //}

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">索引</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs, int pageSize, int pageIndex = 1) where TInfo : ModelInfo, new()
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
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs, Repeater rpt) where TInfo : ModelInfo, new()
        {
            return ToList(lst.Where(o => IDs.Contains(o.ID)), rpt);
        }

        /// <summary>
        ///     获取List列表
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static List<TInfo> ToList<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs) where TInfo : ModelInfo, new()
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
        public static TInfo ToInfo<TInfo>(this IEnumerable<TInfo> lst, int? ID) where TInfo : ModelInfo, new()
        {
            return lst.FirstOrDefault(o => o.ID == ID);
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static int Count<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs) where TInfo : ModelInfo, new()
        {
            return lst.Count(o => IDs.Contains(o.ID));
        }

        /// <summary>
        ///     获取数量
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        public static int Count<TInfo>(this IEnumerable<TInfo> lst, int? ID) where TInfo : ModelInfo, new()
        {
            return lst.Count(o => o.ID == ID);
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        public static bool IsHaving<TInfo>(this IEnumerable<TInfo> lst)
        {
            return lst.Any();
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool IsHaving<TInfo>(this IEnumerable<TInfo> lst, int? ID) where TInfo : ModelInfo, new()
        {
            return lst.Count(o => o.ID == ID) > 0;
        }

        /// <summary>
        ///     判断是否存在记录
        /// </summary>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool IsHaving<TInfo>(this IEnumerable<TInfo> lst, List<int> IDs) where TInfo : ModelInfo, new()
        {
            return lst.Any(o => IDs.Contains(o.ID));
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">ModelInfo</typeparam>
        public static T GetValue<TInfo, T>(this IEnumerable<TInfo> lst, Func<TInfo, T> select, T defValue = default(T))
        {
            if (lst == null) { return defValue; }
            var value = lst.Select(select).FirstOrDefault();
            return value == null ? defValue : value;
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="lst">List列表</param>
        /// <param name="ID">条件，等同于：o=> o.ID == ID 的操作</param>
        /// <param name="select">字段选择器</param>
        /// <param name="defValue">默认值</param>
        /// <typeparam name="T">ModelInfo</typeparam>
        public static T GetValue<TInfo, T>(this IEnumerable<TInfo> lst, int? ID, Func<TInfo, T> select, T defValue = default(T)) where TInfo : ModelInfo, new()
        {
            if (lst == null) { return defValue; }
            lst = lst.Where(o => o.ID == ID).ToList();
            if (lst == null || !lst.Any())
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
            if (lst == null) { return null; }
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
        public static List<T> ToSelectList<TInfo, T>(this IEnumerable<TInfo> lst, List<int> IDs, Func<TInfo, T> select) where TInfo : ModelInfo, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).ToSelectList(select);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<T> ToSelectList<TInfo, T>(this IEnumerable<TInfo> lst, List<int> IDs, int top, Func<TInfo, T> select) where TInfo : ModelInfo, new()
        {
            return lst.ToSelectList(IDs, select).Take(top).ToList();
        }

        /// <summary>
        ///     不重复列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<TInfo> ToDistinctList<TInfo, T>(this IEnumerable<TInfo> lst, Func<TInfo, T> select) where TInfo : ModelInfo, new()
        {
            return lst.Distinct(new InfoComparer<TInfo, T>(select)).ToList();
        }

        /// <summary>
        ///     不重复列表
        /// </summary>
        /// <param name="select">字段选择器</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="lst">列表</param>
        public static List<TInfo> ToDistinctList<TInfo, T>(this IEnumerable<TInfo> lst, List<int> IDs, Func<TInfo, T> select) where TInfo : ModelInfo, new()
        {
            return lst.Where(o => IDs.Contains(o.ID)).Distinct(new InfoComparer<TInfo, T>(select)).ToList();
        }

        #region Cate

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="ID">上级ID</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetSubIDList<TInfo>(this List<TInfo> lstCate, int? ID, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var lst = lstCate.GetSubList(ID, isContainsSub, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID.GetValueOrDefault()).ToList();
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetSubIDList<TInfo>(this List<TInfo> lstCate, string caption, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var lst = lstCate.GetSubList(caption, isContainsSub, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID.GetValueOrDefault()).ToList();
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="ID">上级ID</param>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetSubList<TInfo>(this List<TInfo> lstCate, int? ID, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var lst = new List<TInfo>();
            if (isAddMySelf)
            {
                var info = lstCate.FirstOrDefault(o => o.ID == ID);
                if (info != null) { lst.Add(info); }
            }

            foreach (var info in lstCate.Where(o => o.ParentID == ID).ToList())
            {
                lst.Add(info);
                if (!isContainsSub) { continue; }
                lst.AddRange(lstCate.GetSubList(info.ID, isContainsSub, false));
            }
            return lst;
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetSubList<TInfo>(this List<TInfo> lstCate, string caption, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? new List<TInfo>() : lstCate.GetSubList(info.ID, isContainsSub, isAddMySelf);
        }

        /// <summary>
        ///     通过标题，获取分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isNullAdd">true:不存在则自动创建</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetID<TInfo>(this List<TInfo> lstCate, string caption, bool isNullAdd = false) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetInfo(caption, isNullAdd);
            return info == null ? 0 : info.ID.GetValueOrDefault();
        }

        /// <summary>
        ///     通过标题，获取分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isNullAdd">true:不存在则自动创建</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetInfo<TInfo>(this List<TInfo> lstCate, string caption, bool isNullAdd = false) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.FirstOrDefault(o => o.Caption.IsEquals(caption));
            if (info == null && isNullAdd)
            {
                info = new TInfo { Caption = caption, ParentID = 0 };

                BaseCateModel<TInfo>.Data.Insert(info);
            }
            return info;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetFirstID<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetFirstInfo(ID);
            return info == null ? 0 : info.ID.GetValueOrDefault();
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetFirstInfo<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info == null) { return null; }

            if (lstCate.Count(o => o.ID == info.ParentID) > 0) { info = lstCate.GetFirstInfo(info.ParentID); }

            return info;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetFirstID<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetFirstInfo(caption);
            return info == null ? 0 : info.ID.GetValueOrDefault();
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetFirstInfo<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? null : lstCate.GetFirstInfo(info.ParentID);
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetParentID<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetParentInfo(ID);
            return info == null ? 0 : info.ID.GetValueOrDefault();
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetParentInfo<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info != null) { info = lstCate.FirstOrDefault(o => o.ID == info.ParentID); }
            return info;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetParentID<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetParentInfo(caption);
            return info == null ? 0 : info.ID.GetValueOrDefault();
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetParentInfo<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? null : lstCate.GetParentInfo(info.ID);
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetParentIDList<TInfo>(this List<TInfo> lstCate, int? ID, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var lst = lstCate.GetParentList(ID, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID.GetValueOrDefault()).ToList();
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetParentList<TInfo>(this List<TInfo> lstCate, int? ID, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var lst = new List<TInfo>();
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info == null) { return lst; }

            lst.AddRange(lstCate.GetParentList(info.ParentID, true));
            if (isAddMySelf) { lst.Add(info); }
            return lst;
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetParentIDList<TInfo>(this List<TInfo> lstCate, string caption, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var lst = lstCate.GetParentList(caption, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID.GetValueOrDefault()).ToList();
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetParentList<TInfo>(this List<TInfo> lstCate, string caption, bool isAddMySelf = false) where TInfo : ModelCateInfo, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? new List<TInfo>() : lstCate.GetParentList(info.ID, isAddMySelf);
        }

        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="parentID">所属上级节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int selectedValue, int parentID, bool isUsePrefix = true) where TInfo : ModelCateInfo, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, parentID, 0, null, false, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="where">筛选条件</param>
        /// <param name="isContainsSub">筛选条件是否包含子节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int selectedValue = 0, Func<TInfo, bool> where = null, bool isContainsSub = false, bool isUsePrefix = true) where TInfo : ModelCateInfo, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, 0, 0, where, isContainsSub, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     递归绑定
        /// </summary>
        private static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int parentID, int tagNum, Func<TInfo, bool> where, bool isContainsSub, bool isUsePrefix) where TInfo : ModelCateInfo, new()
        {
            List<TInfo> lst;

            lst = lstCate.FindAll(o => o.ParentID == parentID);
            if (lst == null || lst.Count == 0) { return; }

            if ((parentID == 0 || isContainsSub) && where != null) { lst = lst.Where(where).ToList(); }
            if (lst == null || lst.Count == 0) { return; }

            foreach (var info in lst)
            {
                var text = isUsePrefix ? new string('　', tagNum) + "├─" + info.Caption : info.Caption;

                ddl.Items.Add(new ListItem { Value = info.ID.ToString(), Text = text });
                lstCate.Bind(ddl, info.ID.Value, tagNum + 1, where, isContainsSub, isUsePrefix);
            }
        }
        #endregion

    }
}