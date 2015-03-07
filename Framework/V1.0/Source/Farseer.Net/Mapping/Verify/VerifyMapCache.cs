using System;
using System.Collections.Generic;

namespace FS.Mapping.Verify
{
    /// <summary>
    ///     缓存要验证的实体类
    /// </summary>
    public static class VerifyMapCache
    {
        /// <summary>
        ///     缓存所有验证的实体类
        /// </summary>
        private static readonly Dictionary<Type, VerifyMap> VerifyMapList = new Dictionary<Type, VerifyMap>();

        private static readonly object LockObject = new object();

        /// <summary>
        ///     返回验证的实体类映射的信息
        /// </summary>
        /// <param name="info">IVerification实体类</param>
        public static VerifyMap GetMap<TInfo>(TInfo info) where TInfo : IVerification
        {
            var type = typeof(TInfo);
            return GetMap(type);
        }

        /// <summary>
        ///     返回验证的实体类映射的信息
        /// </summary>
        /// <param name="info">IVerification实体类</param>
        public static VerifyMap GetMap(Type type)
        {
            if (VerifyMapList.ContainsKey(type)) return VerifyMapList[type];
            lock (LockObject)
            {
                if (!VerifyMapList.ContainsKey(type))
                {
                    VerifyMapList.Add(type, new VerifyMap(type));
                }
            }

            return VerifyMapList[type];
        }

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void ClearCache()
        {
            VerifyMapList.Clear();
        }
    }
}