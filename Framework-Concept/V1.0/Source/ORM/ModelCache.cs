using System;
using System.Collections.Generic;

namespace FS.ORM
{
    /// <summary>
    ///     缓存数据库和实体类的映射关系
    /// </summary>
    public static class ModelCache
    {
        /// <summary>
        ///     缓存所有实体类
        /// </summary>
        private static readonly Dictionary<Type, Mapping> ModelList = new Dictionary<Type, Mapping>();

        private static readonly object LockObject = new object();

        /// <summary>
        ///     返回实体类映射的信息
        /// </summary>
        /// <param name="type">实体类Type</param>
        public static Mapping GetInfo(Type type)
        {
            if (!ModelList.ContainsKey(type))
            {
                lock (LockObject)
                {
                    if (!ModelList.ContainsKey(type))
                    {
                        ModelList.Add(type, new Mapping(type));
                    }
                }
            }

            return ModelList[type];
        }

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void ClearCache()
        {
            ModelList.Clear();
        }
    }
}