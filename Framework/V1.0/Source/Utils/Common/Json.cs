using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace FS.Utils.Common
{
    /// <summary>
    /// Json包
    /// </summary>
    public class Json
    {
        #region 私有方法

        /// <summary>
        ///     过滤特殊字符
        /// </summary>
        private static string String2Json(String s)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     格式化字符型、日期型、布尔型
        /// </summary>
        private static string StringFormat(string str, Type type)
        {
            if (type == typeof (string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof (DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof (bool))
            {
                str = str.ToLower();
            }
            else if (type != typeof (string) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
            }
            return str;
        }

        #endregion

        #region List转换成Json

        /// <summary>
        ///     List转换成Json
        /// </summary>
        public static string ListToJson<T>(IList<T> list)
        {
            object obj = list[0];
            return ListToJson(list, obj.GetType().Name);
        }

        /// <summary>
        ///     List转换成Json
        /// </summary>
        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            var Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName)) jsonName = list[0].GetType().Name;
            Json.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var obj = Activator.CreateInstance<T>();
                    var pi = obj.GetType().GetProperties();
                    Json.Append("{");
                    for (var j = 0; j < pi.Length; j++)
                    {
                        var type = pi[j].GetValue(list[i], null).GetType();
                        Json.Append("\"" + pi[j].Name + "\":" +
                                    StringFormat(pi[j].GetValue(list[i], null).ToString(), type));

                        if (j < pi.Length - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < list.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        #endregion

        #region 对象转换为Json

        /// <summary>
        ///     对象转换为Json
        /// </summary>
        /// <param name="jsonObject">对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(object jsonObject)
        {
            var jsonString = "{";
            var propertyInfo = jsonObject.GetType().GetProperties();
            foreach (var property in propertyInfo)
            {
                var objectValue = property.GetGetMethod().Invoke(jsonObject, null);
                var value = string.Empty;
                if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value = "'" + objectValue + "'";
                }
                else if (objectValue is string)
                {
                    value = "'" + ToJson(objectValue.ToString()) + "'";
                }
                else
                {
                    var array = objectValue as IEnumerable;
                    if (array != null)
                    {
                        value = ToJson(array);
                    }
                    else
                    {
                        value = ToJson(objectValue.ToString());
                    }
                }
                jsonString += "\"" + ToJson(property.Name) + "\":" + value + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "}";
        }

        #endregion

        #region 对象集合转换Json

        /// <summary>
        ///     对象集合转换Json
        /// </summary>
        /// <param name="array">集合对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(IEnumerable array)
        {
            var jsonString = array.Cast<object>().Aggregate("[", (current, item) => current + (ToJson(item) + ","));
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }

        #endregion

        #region 普通集合转换Json

        /// <summary>
        ///     普通集合转换Json
        /// </summary>
        /// <param name="array">集合对象</param>
        /// <returns>Json字符串</returns>
        public static string ToArrayString(IEnumerable array)
        {
            var jsonString = "[";
            foreach (var item in array)
            {
                jsonString = ToJson(item.ToString()) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }

        #endregion

        #region  DataSet转换为Json

        /// <summary>
        ///     DataSet转换为Json
        /// </summary>
        /// <param name="dataSet">DataSet对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DataSet dataSet)
        {
            var jsonString = dataSet.Tables.Cast<DataTable>().Aggregate("{", (current, table) => current + ("\"" + table.TableName + "\":" + ToJson(table) + ","));
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }

        #endregion

        #region Datatable转换为Json

        /// <summary>
        ///     Datatable转换为Json
        /// </summary>
        /// <param name="dt">Datatable对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DataTable dt)
        {
            var jsonString = new StringBuilder();
            jsonString.Append("[");
            var drc = dt.Rows;
            for (var i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var strKey = dt.Columns[j].ColumnName;
                    var strValue = drc[i][j].ToString();
                    var type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        /// <summary>
        ///     DataTable转换为Json
        /// </summary>
        public static string ToJson(DataTable dt, string jsonName)
        {
            var Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName)) jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        var type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName + "\":" +
                                    StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        #endregion

        #region DataReader转换为Json

        /// <summary>
        ///     DataReader转换为Json
        /// </summary>
        /// <param name="dataReader">DataReader对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DbDataReader dataReader)
        {
            var jsonString = new StringBuilder();
            jsonString.Append("[");
            while (dataReader.Read())
            {
                jsonString.Append("{");
                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    var type = dataReader.GetFieldType(i);
                    var strKey = dataReader.GetName(i);
                    var strValue = dataReader[i].ToString();
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (i < dataReader.FieldCount - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            dataReader.Close();
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        #endregion
    }
}