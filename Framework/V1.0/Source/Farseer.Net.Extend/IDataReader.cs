using System;
using System.Collections.Generic;
using System.Data;
using FS.Mapping.Table;

namespace FS.Extend
{
    /// <summary>
    ///     Data扩展工具
    /// </summary>
    public static class IDataReaderExtend
    {
        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this IDataReader reader) where T : class
        {
            var list = new List<T>();
            var Map = TableMapCache.GetMap<T>();
            T t;

            while (reader.Read())
            {
                t = (T) Activator.CreateInstance(typeof (T));

                //赋值字段
                foreach (var kic in Map.ModelList)
                {
                    if (reader.GetOrdinal(kic.Value.Column.Name) > -1)
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(t, reader[kic.Value.Column.Name].ConvertType(kic.Key.PropertyType), null);
                    }
                }

                list.Add(t);
            }
            reader.Close();
            reader.Dispose();
            return list;
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="T">实体类</typeparam>
        public static T ToInfo<T>(this IDataReader reader) where T : class
        {
            var map = TableMapCache.GetMap<T>();

            var t = (T) Activator.CreateInstance(typeof (T));
            var isHaveValue = false;

            if (reader.Read())
            {
                //赋值字段
                foreach (var kic in map.ModelList)
                {
                    if (reader.HaveName(kic.Value.Column.Name))
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        kic.Key.SetValue(t, reader[kic.Value.Column.Name].ConvertType(kic.Key.PropertyType), null);
                        isHaveValue = true;
                    }
                }
            }
            reader.Close();
            reader.Dispose();
            return isHaveValue ? t : null;
        }

        /// <summary>
        /// 得到 IDataReader 的Hash
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        static int GetColumnHash(IDataReader reader)
        {
            unchecked
            {
                int colCount = reader.FieldCount, hash = colCount;
                for (int i = 0; i < colCount; i++)
                {   // binding code is only interested in names - not types
                    object tmp = reader.GetName(i);
                    hash = (hash * 31) + (tmp == null ? 0 : tmp.GetHashCode());
                }
                return hash;
            }
        }


        struct DeserializerState
        {
            public readonly int Hash;
            public readonly Func<IDataReader, object> Func;

            public DeserializerState(int hash, Func<IDataReader, object> func)
            {
                Hash = hash;
                Func = func;
            }
        }

        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HaveName(this IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).IsEquals(name)) { return true; }
            }
            return false;
        }
    }
}