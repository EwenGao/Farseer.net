using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FS.Core.Context;
using FS.Mapping.Table;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 数据库提供者（不同数据库的特性）
    /// </summary>
    public abstract class DbProvider
    {
        /// <summary>
        ///     参数前缀
        /// </summary>
        public abstract string ParamsPrefix { get; }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract DbProviderFactory GetDbProviderFactory { get; }

        /// <summary>
        ///     创建字段保护符
        /// </summary>
        /// <param name="fieldName">字符名称</param>
        public abstract string KeywordAegis(string fieldName);

        /// <summary>
        /// 判断是否为字段。还是组合字段。
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public virtual bool IsField(string fieldName)
        {
            return new Regex("^[a-z0-9_-]+$", RegexOptions.IgnoreCase).IsMatch(fieldName.Replace("(", "\\(").Replace(")", "\\)"));
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        /// <param name="output">是否是输出值</param>
        public virtual DbParameter CreateDbParam(string name, object valu, DbType type, int len, bool output = false)
        {
            // 时间类型转换
            if (type == DbType.DateTime)
            {
                DateTime dtValue; DateTime.TryParse(valu.ToString(), out dtValue);
                if (dtValue == DateTime.MinValue) { valu = new DateTime(1900, 1, 1); }
            }
            // 枚举类型转换
            if (valu is Enum) { valu = Convert.ToInt32(valu); len = 8; }

            // List类型转换成字符串并以,分隔
            if (valu.GetType().IsGenericType)
            {
                var sb = new StringBuilder();
                // list类型
                if (valu.GetType().GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    var enumerator = ((IEnumerable)valu).GetEnumerator();
                    while (enumerator.MoveNext()) { sb.Append(enumerator.Current + ","); }
                }
                else
                {
                    if (valu.GetType().GetGenericArguments()[0] == typeof(int))
                    {
                        var enumerator = ((IEnumerable<int?>)valu).GetEnumerator();
                        while (enumerator.MoveNext()) { sb.Append(enumerator.Current.GetValueOrDefault() + ","); }
                    }
                }
                if (sb.Length > 0) { valu = sb.Remove(sb.Length - 1, 1).ToString(); }
                len = valu.ToString().Length;
            }

            var param = GetDbProviderFactory.CreateParameter();
            param.DbType = type;
            param.ParameterName = ParamsPrefix + name;
            param.Value = valu;
            if (len > 0) param.Size = len;
            if (output) param.Direction = ParameterDirection.Output;
            return param;
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        public virtual DbParameter CreateDbParam(string name, object valu)
        {
            if (valu == null) { valu = string.Empty; }

            var type = valu.GetType();
            if (type.Name.Equals("Nullable`1"))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            switch (type.Name)
            {
                case "DateTime":
                    return CreateDbParam(name, valu, DbType.DateTime, 8, false);
                case "Boolean":
                    return CreateDbParam(name, valu, DbType.Boolean, 1, false);
                case "Int32":
                    return CreateDbParam(name, valu, DbType.Int32, 4, false);
                case "Int16":
                    return CreateDbParam(name, valu, DbType.Int16, 2, false);
                case "Decimal":
                    return CreateDbParam(name, valu, DbType.Decimal, 8, false);
                case "Byte":
                    return CreateDbParam(name, valu, DbType.Byte, 1, false);
                case "Long":
                    return CreateDbParam(name, valu, DbType.Decimal, 8, false);
                case "Float":
                    return CreateDbParam(name, valu, DbType.Decimal, 8, false);
                case "Double":
                    return CreateDbParam(name, valu, DbType.Decimal, 8, false);
                default:
                    return CreateDbParam(name, valu, DbType.String, valu.ToString().Length, false);
            }
        }

        /// <summary>
        ///     获取该实体类的参数
        /// </summary>
        /// <param name="info">实体类</param>
        public virtual IEnumerable<DbParameter> GetParameter<TEntity>(TEntity info) where TEntity : class,new()
        {
            var map = TableMapCache.GetMap(info);
            var lst = new List<DbParameter>();

            foreach (var kic in map.ModelList.Where(o => o.Value.IsDbField))
            {
                var obj = kic.Key.GetValue(info, null);
                if (obj == null || obj is TableSet<TEntity>) { continue; }

                lst.Add(CreateDbParam(kic.Value.Column.Name, obj));
            }

            return lst;
        }
    }
}
