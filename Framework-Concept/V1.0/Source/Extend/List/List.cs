using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FS.Core.Data;
using FS.Core.Model;
using FS.ORM;
using FS.Utils.Common;
using System.Data;

namespace FS.Extend
{
    /// <summary>
    ///     Data扩展工具
    /// </summary>
    public static partial class ListExtend
    {
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
        /// <typeparam name="T">任何对像</typeparam>
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
        public static List<TInfo> TestData<TInfo>(this List<TInfo> lst, int count) where TInfo : BaseInfo, new()
        {
            lst = new List<TInfo>();
            for (int i = 0; i < count; i++)
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
            for (int index = 0; index < lst.Count; index++) // 迭代所有关键词
            {
                var key = lst[index];
                for (int moveIndex = 0; moveIndex < key.Length; moveIndex += 1)     // 每次移动后2位当前关键词
                {
                    for (int step = key.Length; (step - moveIndex) >= 2; step--)   // 每次减少1位来对比
                    {
                        var clearKey = key.Substring(moveIndex, step - moveIndex);  // 截取的关键词

                        for (int index2 = index + 1; index2 < lst.Count; index2++)  // 清除下一项的所有字符串
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
        public static DataTable ToDataTable<TInfo>(this List<TInfo> lst) where TInfo : BaseInfo, new()
        {
            var dt = new DataTable();
            if (lst.Count == 0) { return dt; }
            var map = ModelCache.GetInfo(lst[0].GetType());
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
    }
}