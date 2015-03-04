using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;
using FS.Core.Model;
using FS.Interface;
using FS.Mapping.Table;
using FS.Utils.Common;
using FS.Utils.Web;

namespace FS.Extend
{
    /// <summary>
    ///     扩展工具
    /// </summary>
    public static class ExtendExtend
    {
        /// <summary>
        ///     把服務器返回的Cookies信息寫入到客戶端中
        /// </summary>
        public static void Cookies(this WebClient wc)
        {
            if (wc.ResponseHeaders == null) return;
            var setcookie = wc.ResponseHeaders[HttpResponseHeader.SetCookie];
            if (string.IsNullOrEmpty(setcookie)) return;
            var cookie = wc.Headers[HttpRequestHeader.Cookie];
            var cookieList = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(cookie))
            {
                foreach (var ck in cookie.Split(';'))
                {
                    var key = ck.Substring(0, ck.IndexOf('='));
                    var value = ck.Substring(ck.IndexOf('=') + 1);
                    if (!cookieList.ContainsKey(key)) cookieList.Add(key, value);
                }
            }

            foreach (var ck in setcookie.Split(';'))
            {
                var str = ck;
                while (str.Contains(',') && str.IndexOf(',') < str.LastIndexOf('='))
                {
                    str = str.Substring(str.IndexOf(',') + 1);
                }
                var key = str.IndexOf('=') != -1 ? str.Substring(0, str.IndexOf('=')) : "";
                var value = str.Substring(str.IndexOf('=') + 1);
                if (!cookieList.ContainsKey(key))
                    cookieList.Add(key, value);
                else
                    cookieList[key] = value;
            }

            var list = new string[cookieList.Count()];
            var index = 0;
            foreach (var pair in cookieList)
            {
                list[index] = string.Format("{0}={1}", pair.Key, pair.Value);
                index++;
            }

            wc.Headers[HttpRequestHeader.Cookie] = list.ToString(";");
        }

        /// <summary>
        ///     将XML转成实体
        /// </summary>
        public static List<T> ToList<T>(this XElement element) where T : ModelInfo
        {
            var orm = TableMapCache.GetMap<T>();
            var list = new List<T>();
            Type type;

            T t;

            foreach (var el in element.Elements())
            {
                t = (T)Activator.CreateInstance(typeof(T));

                //赋值字段
                foreach (var kic in orm.ModelList)
                {
                    type = kic.Key.PropertyType;
                    if (!kic.Key.CanWrite) { continue; }
                    if (kic.Value.PropertyExtend == eumPropertyExtend.Attribute)
                    {
                        if (el.Attribute(kic.Value.Column.Name) == null) { continue; }
                        kic.Key.SetValue(t, el.Attribute(kic.Value.Column.Name).Value.ConvertType(type), null);
                    }
                    else if (kic.Value.PropertyExtend == eumPropertyExtend.Element)
                    {
                        if (el.Element(kic.Value.Column.Name) == null) { continue; }
                        kic.Key.SetValue(t, el.Element(kic.Value.Column.Name).Value.ConvertType(type), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        ///     将DataRowCollection转成List[DataRow]
        /// </summary>
        /// <param name="drc">DataRowCollection</param>
        public static List<DataRow> ToRows(this DataRowCollection drc)
        {
            var lstRow = new List<DataRow>();

            if (drc == null) { return lstRow; }

            foreach (DataRow dr in drc) { lstRow.Add(dr); }

            return lstRow;
        }

        /// <summary>
        ///     将DataRow转成实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dr">源DataRow</param>
        public static T ToInfo<T>(this DataRow dr) where T : ModelInfo, new()
        {
            var map = TableMapCache.GetMap<T>();
            var t = (T)Activator.CreateInstance(typeof(T));

            //赋值字段
            foreach (var kic in map.ModelList)
            {
                if (dr.Table.Columns.Contains(kic.Value.Column.Name))
                {
                    if (!kic.Key.CanWrite) { continue; }
                    kic.Key.SetValue(t, dr[kic.Value.Column.Name].ConvertType(kic.Key.PropertyType), null);
                }
            }
            return t ?? new T();
        }

        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="ds">源DataSet</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this DataSet ds) where T : ModelInfo, new()
        {
            return ds.Tables.Count == 0 ? null : ds.Tables[0].ToList<T>();
        }

        /// <summary>
        ///     扩展 Dictionary 根据Value反向查找Key的方法
        /// </summary>
        /// <param name="list">Dictionary对象</param>
        /// <param name="t2">键值</param>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        public static T1 GetKey<T1, T2>(this Dictionary<T1, T2> list, T2 t2)
        {
            foreach (var obj in list)
            {
                if (obj.Value.Equals(t2)) return obj.Key;
            }
            return default(T1);
        }

        /// <summary>
        ///     检查是否存在该类型的子窗体
        /// </summary>
        /// <param name="form">Windows窗体对象</param>
        /// <param name="childFormName">窗体名称</param>
        /// <returns>是否存在</returns>
        public static bool IsExist(this Form form, string childFormName)
        {
            foreach (var frm in form.MdiChildren)
            {
                if (frm.GetType().Name == childFormName)
                {
                    frm.Activate();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     设置创建人信息
        /// </summary>
        /// <param name="createInfo">被赋值的实体</param>
        /// <param name="createID">创建人ID</param>
        /// <param name="createName">创建人名称</param>
        public static void SetCreateInfo(this ICreateInfo createInfo, int? createID = 0, string createName = "")
        {
            createInfo.CreateIP = Req.GetIP();
            createInfo.CreateAt = DateTime.Now;
            createInfo.CreateID = createID;
            createInfo.CreateName = createName;
        }

        /// <summary>
        ///     设置修改人信息
        /// </summary>
        /// <param name="updateInfo">被赋值的实体</param>
        /// <param name="updateID">创建人ID</param>
        /// <param name="updateName">创建人名称</param>
        public static void SetUpdateInfo(this IUpdateInfo updateInfo, int? updateID = 0, string updateName = "")
        {
            updateInfo.UpdateIP = Req.GetIP();
            updateInfo.UpdateAt = DateTime.Now;
            updateInfo.UpdateID = updateID;
            updateInfo.UpdateName = updateName;
        }

        /// <summary>
        ///     设置审核人信息
        /// </summary>
        /// <param name="auditInfo">被赋值的实体</param>
        /// <param name="auditID">创建人ID</param>
        /// <param name="auditName">创建人名称</param>
        public static void SetAuditInfo(this IAuditInfo auditInfo, int? auditID = 0, string auditName = "")
        {
            auditInfo.AuditIP = Req.GetIP();
            auditInfo.AuditAt = DateTime.Now;
            auditInfo.AuditID = auditID;
            auditInfo.AuditName = auditName;
        }

        /// <summary>
        ///     Func 转换成 Predicate 对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="source">源Func对象</param>
        public static Predicate<T> ToPredicate<T>(this Func<T, bool> source) where T : ModelInfo, new()
        {
            return new Predicate<T>(source);
        }

        /// <summary>
        ///     转换成Json格式 x=x&x=x
        /// </summary>
        /// <param name="dic">Dictionary对象</param>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        public static string ToJson<T1, T2>(this Dictionary<T1, T2> dic)
        {
            var sp = new StrPlus();
            foreach (var item in dic)
            {
                sp += string.Format("{0}={1}&", item.Key, item.Value);
            }
            return sp.DelLastChar("&");
        }
    }
}