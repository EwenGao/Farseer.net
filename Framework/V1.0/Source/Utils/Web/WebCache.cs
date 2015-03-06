using System;
using System.Web;
using System.Web.Caching;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     对缓存操作进行封装
    /// </summary>
    public abstract class WebCache
    {
        private static readonly Cache webCache = HttpRuntime.Cache;

        /// <summary>
        ///     添加对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeOut">默认缓存存活期为1440分钟(24小时)单位：／分钟</param>
        public static void Add<T>(string key, T t, int timeOut = 1440)
        {
            if (!string.IsNullOrEmpty(key) && t != null)
            {
                webCache.Insert(key, t, null, DateTime.Now.AddMinutes(timeOut), Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        ///     返回对象
        /// </summary>
        public static T Get<T>(string key)
        {
            return string.IsNullOrEmpty(key) ? default(T) : webCache.Get(key).ConvertType<T>();
        }

        /// <summary>
        ///     返回对象
        /// </summary>
        public static object Get(string key)
        {
            return Get<object>(key);
        }

        /// <summary>
        ///     删除对象
        /// </summary>
        public static void Clear(string key)
        {
            if (string.IsNullOrEmpty(key)) { return; }
            webCache.Remove(key);
        }
    }
}