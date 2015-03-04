using System;
using System.Collections.Generic;

namespace FS.Mapping.Table
{
    /// <summary>
    ///     缓存数据库和实体类的映射关系
    /// </summary>
    public static class TableMapCache
    {
        /// <summary>
        ///     缓存所有实体类
        /// </summary>
        private static readonly Dictionary<Type, TableMap> MapList = new Dictionary<Type, TableMap>();

        private static readonly object LockObject = new object();

        /// <summary>
        ///     返回实体类映射的信息
        /// </summary>
        /// <param name="info">ModelInfo实体类</param>
        public static TableMap GetMap<TInfo>(TInfo info = null) where TInfo : class
        {
            var type = typeof (TInfo);
            return GetMap(type);
        }

        /// <summary>
        ///     返回实体类映射的信息
        /// </summary>
        public static TableMap GetMap(Type type)
        {
            if (MapList.ContainsKey(type)) return MapList[type];
            lock (LockObject)
            {
                if (!MapList.ContainsKey(type))
                {
                    MapList.Add(type, new TableMap(type));
                }
            }

            return MapList[type];
        }

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void ClearCache()
        {
            MapList.Clear();
        }
    }
}