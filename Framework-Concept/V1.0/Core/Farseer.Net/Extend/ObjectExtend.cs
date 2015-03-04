using System;
using System.Text;

namespace FS.Extend
{
    /// <summary>
    ///     Object扩展工具
    /// </summary>
    public static class ObjectExtend
    {
        /// <summary>
        ///     将任何数组转换成用符号连接的字符串
        /// </summary>
        /// <param name="obj">任何对象</param>
        /// <param name="func">传入要在转换过程中要执行的方法</param>
        /// <param name="sign">分隔符</param>
        /// <typeparam name="T">基本对象</typeparam>
        public static string ToString<T>(this T[] obj, string sign = ",", Func<T, string> func = null)
        {
            if (obj == null || obj.Length == 0)
            {
                return string.Empty;
            }

            var str = new StringBuilder();
            foreach (var t in obj)
            {
                if (func == null)
                {
                    str.Append(sign + t);
                }
                else
                {
                    str.Append(sign + func(t));
                }
            }

            return str.ToString().Substring(sign.Length);
        }

        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue">要转换的源对象</param>
        /// <param name="defValue">转换失败时，代替的默认值</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static T ConvertType<T>(this object sourceValue, T defValue = default(T))
        {
            if (sourceValue == null) { return defValue; }

            var returnType = typeof(T);
            var sourceType = sourceValue.GetType();

            // 相同类型，则直接返回原型
            if (Type.GetTypeCode(returnType) == Type.GetTypeCode(sourceType)) { return (T)sourceValue; }

            var val = sourceValue.ConvertType(returnType);
            return val != null ? (T)val : defValue;
        }

        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="defType">要转换的对象的类型</param>
        public static object ConvertType(this object sourceValue, Type defType)
        {
            if (sourceValue == null) { return null; }

            // 对   Nullable<> 类型处理
            if (defType.IsGenericType && defType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return sourceValue.ConvertType(Nullable.GetUnderlyingType(defType)); }
            // 对   List 类型处理
            if (defType.IsGenericType && defType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                var objString = sourceValue.ToString();
                // List参数类型
                var argumType = defType.GetGenericArguments()[0];

                switch (Type.GetTypeCode(argumType))
                {
                    case TypeCode.Boolean: { return objString.ToList(false); }
                    case TypeCode.DateTime: { return objString.ToList(DateTime.MinValue); }
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single: { return objString.ToList(0m); }
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16: { return objString.ToList(UInt16.MinValue); }
                    case TypeCode.UInt32: { return objString.ToList(UInt32.MinValue); }
                    case TypeCode.UInt64: { return objString.ToList(UInt64.MinValue); }
                    case TypeCode.Int16: { return objString.ToList((Int16)0); }
                    case TypeCode.Int64: { return objString.ToList((Int64)0); }
                    case TypeCode.Int32: { return objString.ToList(0); }
                    case TypeCode.Empty:
                    case TypeCode.Char:
                    case TypeCode.String: { return objString.ToList(""); }
                }
            }

            return ConvertType(sourceValue, sourceValue.GetType(), defType);
        }

        /// <summary>
        /// 将值转换成类型对象的值（此方法作为公共的调用，只支持单值转换)
        /// </summary>
        /// <param name="objValue">要转换的值</param>
        /// <param name="objType">要转换的值类型</param>
        /// <param name="defType">转换失败时，代替的默认值类型</param>
        /// <returns></returns>
        private static object ConvertType(object objValue, Type objType, Type defType)
        {
            if (objValue == null) { return null; }
            if (objType == defType) { return objValue;}

            var objString = objValue.ToString();

            var defTypeCode = Type.GetTypeCode(defType);
            var objTypeCode = Type.GetTypeCode(objType);

            // 枚举处理
            if (defType.IsEnum)
            {
                if (objString.IsNullOrEmpty()) { return null; }
                return objString.IsType<int>() ? Enum.ToObject(defType, int.Parse(objString)) : Enum.Parse(defType, objString, true);
            }
            // bool转int
            if (objTypeCode == TypeCode.Boolean)
            {
                switch (defTypeCode)
                {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64: return (object)(objValue.ConvertType(true) ? 1 : 0);
                }
            }

            switch (defTypeCode)
            {
                case TypeCode.Boolean: { return (object)(objString.IsHaving() && (objString.IsEquals("on") || objString == "1" || objString.IsEquals("true"))); }
                case TypeCode.Byte: { Byte result; return Byte.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Char: { Char result; return Char.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.DateTime: { DateTime result; return DateTime.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Decimal: { Decimal result; return Decimal.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Double: { Double result; return Double.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Int16: { Int16 result; return Int16.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Int32: { Int32 result; return Int32.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Int64: { Int64 result; return Int64.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.SByte: { SByte result; return SByte.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Single: { Single result; return Single.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.UInt16: { UInt16 result; return UInt16.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.UInt32: { UInt32 result; return UInt32.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.UInt64: { UInt64 result; return UInt64.TryParse(objString, out result) ? (object)result : null; }
                case TypeCode.Empty:
                case TypeCode.String: { return (object)objString; }
                case TypeCode.Object: { break; }
            }

            try { return Convert.ChangeType(objValue, defType); }
            catch { return null; }
        }

        /// <summary>
        ///     判断是否T类型
        /// </summary>
        /// <param name="objValue">要判断的对象</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static bool IsType<T>(this object objValue)
        {
            if (objValue == null) { return false; }

            var defType = typeof(T);
            var objType = objValue.GetType();

            var defTypeCode = Type.GetTypeCode(defType);
            var objTypeCode = Type.GetTypeCode(objType);

            // 相同类型，则直接返回原型
            if (objTypeCode == defTypeCode) { return true; }

            // 判断是否为泛型
            if (defType.IsGenericType)
            {
                // 非 Nullable<> 类型
                if (defType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    return objValue is T;
                }
                // 对Nullable<>类型处理
                defType = Nullable.GetUnderlyingType(defType);
                defTypeCode = Type.GetTypeCode(defType);
            }

            if (defType.IsEnum) { return objValue is Enum; }
            var objString = objValue.ToString();

            switch (defTypeCode)
            {
                case TypeCode.Boolean: { return !objString.IsNullOrEmpty() && (objString.IsEquals("on") || objString == "1" || objString.IsEquals("true")); }
                case TypeCode.Byte: { Byte result; return Byte.TryParse(objString, out result); }
                case TypeCode.Char: { Char result; return Char.TryParse(objString, out result); }
                case TypeCode.DateTime: { DateTime result; return DateTime.TryParse(objString, out result); }
                case TypeCode.Decimal: { Decimal result; return Decimal.TryParse(objString, out result); }
                case TypeCode.Double: { Double result; return Double.TryParse(objString, out result); }
                case TypeCode.Int16: { Int16 result; return Int16.TryParse(objString, out result); }
                case TypeCode.Int32: { Int32 result; return Int32.TryParse(objString, out result); }
                case TypeCode.Int64: { Int64 result; return Int64.TryParse(objString, out result); }
                case TypeCode.SByte: { SByte result; return SByte.TryParse(objString, out result); }
                case TypeCode.Single: { Single result; return Single.TryParse(objString, out result); }
                case TypeCode.UInt16: { UInt16 result; return UInt16.TryParse(objString, out result); }
                case TypeCode.UInt32: { UInt32 result; return UInt32.TryParse(objString, out result); }
                case TypeCode.UInt64: { UInt64 result; return UInt64.TryParse(objString, out result); }
                case TypeCode.Empty:
                case TypeCode.String: { return true; }
                case TypeCode.Object: { break; }
            }
            return objType == defType;
        }

        /// <summary>
        /// 直接返回分隔后的元素项
        /// </summary>
        /// <param name="arr">要分隔的字符串</param>
        /// <param name="tag">分隔的标识</param>
        /// <param name="index">元素下标</param>
        /// <returns></returns>
        public static string Split(this string arr, string tag, int index)
        {
            var arrs = arr.Split(tag);
            if (arrs.Length <= index) { return string.Empty; }
            return arrs[index];
        }

        /// <summary>
        /// 直接返回分隔后的元素项
        /// </summary>
        /// <param name="arr">要分隔的字符串</param>
        /// <param name="tag">分隔的标识</param>
        /// <param name="index">元素下标</param>
        /// <returns></returns>
        public static string Split(this string arr, char tag, int index)
        {
            var arrs = arr.Split(tag);
            if (arrs.Length <= index) { return string.Empty; }
            return arrs[index];
        }
    }
}