using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static class EnumExtend
    {
        private static readonly object objLock = new object();

        /// <summary>
        ///     缓存列表
        /// </summary>
        public static Dictionary<string, string> dicEnum = new Dictionary<string, string>();

        /// <summary>
        ///     获取枚举中文
        /// </summary>
        /// <param name="eum">枚举值</param>
        public static string GetName(this Enum eum)
        {
            if (eum == null)
            {
                return "";
            }
            var enumType = eum.GetType();
            var enumName = eum.ToString();
            var key = string.Format("{0}.{1}", enumType.FullName, enumName);

            if (dicEnum.ContainsKey(key))
            {
                return dicEnum[key];
            }

            foreach (var fieldInfo in enumType.GetFields())
            {
                //判断名称是否相等   
                if (fieldInfo.Name != enumName) continue;

                //反射出自定义属性   
                foreach (Attribute attr in fieldInfo.GetCustomAttributes(true))
                {
                    //类型转换找到一个Description，用Description作为成员名称

                    var dscript = attr as DisplayAttribute;
                    if (dscript != null)
                    {
                        if (!dicEnum.ContainsKey(key))
                        {
                            lock (objLock)
                            {
                                if (!dicEnum.ContainsKey(key))
                                {
                                    dicEnum.Add(key, dscript.Name);
                                }
                            }
                        }
                        return dscript.Name;
                    }
                }
            }

            //如果没有检测到合适的注释，则用默认名称   
            return enumName;
        }

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static Dictionary<int, string> ToDictionary(this Type enumType)
        {
            var dic = new Dictionary<int, string>();
            foreach (int value in Enum.GetValues(enumType))
            {
                dic.Add(value, GetName((Enum) Enum.ToObject(enumType, value)));
            }
            return dic;
        }

        /// <summary>
        ///     枚举转ListItem
        /// </summary>
        public static List<ListItem> ToListItem(this Type enumType)
        {
            var lst = new List<ListItem>();

            foreach (int value in Enum.GetValues(enumType))
            {
                var listitem = new ListItem(GetName((Enum) Enum.ToObject(enumType, value)), value.ToString());
                lst.Add(listitem);
            }

            return lst;
        }

        /// <summary>
        ///     枚举转ListItem
        /// </summary>
        public static List<SelectListItem> ToSelectListItem(this Type enumType)
        {
            var lst = new List<SelectListItem>();
            foreach (int value in Enum.GetValues(enumType))
            {
                lst.Add(new SelectListItem
                            {
                                Value = value.ToString(),
                                Text = GetName((Enum) Enum.ToObject(enumType, value))
                            });
            }
            return lst;
        }
    }
}