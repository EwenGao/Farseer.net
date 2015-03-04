using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using FS.Extend;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库提供者
    /// </summary>
    public class DbProvider
    {
        /// <summary>
        ///     参数前缀
        /// </summary>
        public virtual string ParamsPrefix
        {
            get { return "@"; }
        }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public virtual DbProviderFactory GetDbProviderFactory { get; set; }

        /// <summary>
        ///     创建字段保护符
        /// </summary>
        /// <param name="fieldName">字符名称</param>
        public virtual string CreateTableAegis(string fieldName)
        {
            // 如果不是字段名，则直接返回
            if (!IsField(fieldName)) { return fieldName; }

            return string.Format("[{0}]", fieldName);
        }

        /// <summary>
        /// 判断是否为字段。还是组合字段。
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
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
            if (type == DbType.DateTime && valu.ConvertType(DateTime.MinValue) == DateTime.MinValue)
            {
                valu = "1900-01-01".ConvertType<DateTime>();
            }

            if (valu is Enum)
            {
                valu = (int)valu;
                len = 8;
            }
            if (valu.GetType().IsGenericType)
            {
                if (valu.GetType().GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    valu = ((IEnumerable)valu).ToString(",");
                }
                else
                {
                    if (valu.GetType().GetGenericArguments()[0] == typeof(int))
                    {
                        valu = ((IEnumerable<int?>)valu).ToString(",");
                    }
                }
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
            if (type.Name.IsEquals("Nullable`1"))
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
    }
}