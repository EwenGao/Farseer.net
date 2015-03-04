using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FS.Configs;
using FS.Extend;
using FS.Utils.Common;
using FS.Utils.Web;

namespace FS.Core.Model
{
    /// <summary>
    /// 片断缓存操作
    /// </summary>
    public class DataCache
    {
        /// <summary>
        ///     数据类型
        /// </summary>
        public enum enumDataType
        {
            /// <summary>
            ///     列表
            /// </summary>
            [Display(Name = "列表")]
            List,
            /// <summary>
            ///     数量
            /// </summary>
            [Display(Name = "数量")]
            Count,
            /// <summary>
            ///     实体
            /// </summary>
            [Display(Name = "实体")]
            Info,
            /// <summary>
            ///     xxxx
            /// </summary>
            [Display(Name = "表格")]
            Table,
            /// <summary>
            ///     xxxx
            /// </summary>
            [Display(Name = "随机表格")]
            TableByRand,
            /// <summary>
            ///     随机列表
            /// </summary>
            [Display(Name = "随机列表")]
            ListByRand,
            /// <summary>
            ///     总数
            /// </summary>
            [Display(Name = "总数")]
            Sum,
            /// <summary>
            ///     最大数
            /// </summary>
            [Display(Name = "最大数")]
            Max,
            /// <summary>
            ///     最小值
            /// </summary>
            [Display(Name = "最小值")]
            Min,
            /// <summary>
            ///     单值
            /// </summary>
            [Display(Name = "单值")]
            Value,
            /// <summary>
            ///     存值
            /// </summary>
            [Display(Name = "存值")]
            IsHaving,
        }

        static DataCache()
        {
            DataStatisticsList = new List<DataCache>();
        }

        private DataCache()
        {
        }

        /// <summary>
        ///     所有统计对象
        /// </summary>
        public static List<DataCache> DataStatisticsList { get; set; }

        /// <summary>
        ///     数据类型
        /// </summary>
        public enumDataType DataType { get; internal set; }

        /// <summary>
        ///     缓存Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        ///     统计时间
        /// </summary>
        public List<DateTime> LstActiveAt { get; internal set; }

        /// <summary>
        ///     加入缓存的时间
        /// </summary>
        public DateTime JoinCacheAt { get; internal set; }

        /// <summary>
        ///     缓存值
        /// </summary>
        public object Value
        {
            get { return WebCache.Get(Key); }
        }

        /// <summary>
        ///     缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private T GetValue<T>()
        {
            return WebCache.Get<T>(Key);
        }

        /// <summary>
        ///     获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">dbBuilder.ToString()：唯一标识</param>
        public static T Get<T>(string sql, enumDataType dataType, Func<T> func = null, bool isIgnoreStatistics = false)
        {
            var key = CreateKey(sql, dataType);

            var value = DataStatisticsList.FirstOrDefault(o => o.Key == key).Value;
            if (value == null)
            {
                if (func == null) { return default(T); }
                value = func();
                Set(key, value, isIgnoreStatistics);
            }
            return (T)value;
        }

        /// <summary>
        ///     添加缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t">dbBuilder.ToString()：唯一标识</param>
        /// <param name="isIgnoreStatistics">是否忽略统计，直接加入缓存</param>
        private static void Set<T>(string key, T t, bool isIgnoreStatistics = false)
        {
            var cache = DataStatisticsList.FirstOrDefault(o => o.Key == key);
            if (!isIgnoreStatistics && cache.LstActiveAt.Count < CacheConfigs.ConfigInfo.VisitCount) { return; }

            // 加入缓存
            WebCache.Add(key, t, CacheConfigs.ConfigInfo.CacheTimeOut);
            cache.JoinCacheAt = DateTime.Now;
        }

        /// <summary>
        ///     添加统计
        /// </summary>
        /// <param name="sql">dbBuilder.ToString()：唯一标识</param>
        /// <param name="dataType">数据类型</param>
        private static string CreateKey(string sql, enumDataType dataType)
        {
            #region 移除失效时间
            // 移除失效的时间
            DataStatisticsList.ForEach(o => o.LstActiveAt.RemoveAll(t => DateTime.Now > t.AddMinutes(CacheConfigs.ConfigInfo.VisitTime)));

            // 移除无时间统计的项
            DataStatisticsList.RemoveAll(o => o.Value == null && o.LstActiveAt.Count() == 0);
            // 重置加入缓存时间
            var lst = DataStatisticsList.Where(o => o.Value == null).ToList();
            for (var i = 0; i < lst.Count; i++) { lst[i].JoinCacheAt = DateTime.MinValue; }
            #endregion

            var key = Encrypt.MD5(dataType.GetName() + sql).Substring(8, 16);

            // 加入统计
            var cache = DataStatisticsList.FirstOrDefault(o => o.Key == key);
            if (cache == null)
            {
                cache = new DataCache { Key = key, DataType = dataType, LstActiveAt = new List<DateTime>() };
                DataStatisticsList.Add(cache);
            }
            cache.LstActiveAt.Add(DateTime.Now);

            return key;
        }

        /// <summary>
        ///     清除统计、缓存
        /// </summary>
        public static void Clear()
        {
            foreach (var item in DataStatisticsList) { WebCache.Clear(item.Key); }
            DataStatisticsList.Clear();
        }
    }
}