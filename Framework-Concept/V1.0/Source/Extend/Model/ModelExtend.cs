using FS.Core.Model;
using FS.ORM;
using FS.Utils.Common;
using FS.Utils.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     实体类扩展方法
    /// </summary>
    public static class ModelExtend
    {
        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="info">要填入数据的实体类</param>
        /// <param name="contentPlaceHolderID">母版页面版ID</param>
        /// <param name="prefix">控件前缀</param>
        public static void Fill<TInfo>(this TInfo info, Page page, string contentPlaceHolderID, string prefix = "hl") where TInfo : class, new()
        {
            if (info == null) { return; }

            var masterControl = page.FindControl(contentPlaceHolderID);
            if (masterControl == null) { return; }

            Fill(masterControl.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="info">要填充的值</param>
        public static void Fill<TInfo>(this TInfo info, Page page, string prefix = "hl") where TInfo : class, new()
        {
            if (info == null) { return; }
            Fill(page.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="controls">页面控件集合</param>
        /// <param name="infoValue">所属实体类的值</param>
        /// <param name="prefix">前缀</param>
        private static void Fill(ControlCollection controls, object infoValue, string prefix = "hl")
        {
            if (infoValue == null || controls == null)
            {
                return;
            }
            var map = ModelCache.GetInfo(infoValue.GetType());
            foreach (var kic in map.ModelList)
            {
                // 当前成员的值
                var value = kic.Key.GetValue(infoValue, null);
                if (value == null) { continue; }

                var type = value.GetType();

                // 当前成员，是一个类
                if (value is BaseInfo)
                {
                    foreach (var item in type.GetProperties()) { Fill(controls, value, prefix); }
                    continue;
                }

                foreach (Control item in controls)
                {
                    var control = item.FindControl(prefix + kic.Key.Name);
                    if (control == null) { continue; }

                    if (control is HiddenField)
                    {
                        ((HiddenField)control).Value = value.ToString();
                        break;
                    }
                    if (control is CheckBox) { ((CheckBox)control).Checked = value.ConvertType(false); break; }
                    if (control is CheckBoxList)
                    {
                        // 数据保存的是数字以逗号分隔的数据，并且是ListControl的控件，则可以直接填充数据
                        if (value is string)
                        {
                            var lstIDs = value.ToString().ToList(0);
                            ((CheckBoxList)control).SetValue(lstIDs);
                            break;
                        }

                        // 枚举为二进制时
                        var types = kic.Key.PropertyType.GetGenericArguments();
                        if (types != null && types.Length > 0 && types[0].IsEnum)
                        {
                            var att = types[0].GetCustomAttributes(typeof(FlagsAttribute), false);

                            if (att != null && att.Length > 0)
                            {
                                foreach (ListItem listItem in ((CheckBoxList)control).Items)
                                {
                                    var itemValue = listItem.Value.ConvertType(0);
                                    listItem.Selected = (value.ConvertType(0) & itemValue) == itemValue;
                                }
                                break;
                            }
                        }

                    }
                    if (control is ListControl)
                    {
                        ((ListControl)control).SelectedItems(value);
                        break;

                    }

                    if (value is Enum) { value = ((Enum)value).GetName(); }
                    if (value is IList) { value = ((IList)value).ToString(","); }
                    if (value is bool) { value = ((bool)value) ? "是" : "否"; }

                    if (control is TextBox) { ((TextBox)control).Text = value.ToString(); break; }
                    if (control is Label) { ((Label)control).Text = value.ToString(); break; }
                    if (control is Literal) { ((Literal)control).Text = value.ToString(); break; }
                    if (control is Image) { ((Image)control).ImageUrl = value.ToString(); break; }
                    if (control is HyperLink) { ((HyperLink)control).NavigateUrl = value.ToString(); break; }
                }
            }
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TInfo>(this TInfo info, Action<string, string> tip = null, string url = "") where TInfo : class, new()
        {
            if (info == null)
            {
                return false;
            }
            if (tip == null)
            {
                tip = new Terminator().Alert;
            }
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = info.Check(out dicError);

            if (!result)
            {
                var lst = new List<string>();
                foreach (var item in dicError)
                {
                    lst.AddRange(item.Value);
                }

                tip(lst.ToString("<br />"), url);
            }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TInfo>(this TInfo info, Action<Dictionary<string, List<string>>> tip) where TInfo : class, new()
        {
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = info.Check(out dicError);

            if (!result)
            {
                tip(dicError);
            }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError">返回错误消息,key：属性名称；vakue：错误消息</param>
        /// <param name="info">要检测的实体</param>
        public static bool Check<TInfo>(this TInfo info, out Dictionary<string, List<string>> dicError) where TInfo : class, new()
        {
            dicError = new Dictionary<string, List<string>>();
            var map = ModelCache.GetInfo(info.GetType());
            foreach (var kic in map.ModelList)
            {
                var lstError = new List<string>();
                var value = kic.Key.GetValue(info, null);

                // 是否必填
                if (kic.Value.Required != null && !kic.Value.Required.IsValid(value))
                {
                    lstError.Add(kic.Value.Required.ErrorMessage);
                }

                if (value == null)
                {
                    continue;
                }

                // 字符串长度判断
                if (kic.Value.StringLength != null && !kic.Value.StringLength.IsValid(value))
                {
                    lstError.Add(kic.Value.StringLength.ErrorMessage);
                }

                // 值的长度
                if (kic.Value.Range != null && !kic.Value.Range.IsValid(value))
                {
                    lstError.Add(kic.Value.Range.ErrorMessage);
                }

                // 正则
                if (kic.Value.RegularExpression != null && !kic.Value.RegularExpression.IsValid(value))
                {
                    lstError.Add(kic.Value.RegularExpression.ErrorMessage);
                }

                if (lstError.Count > 0)
                {
                    dicError.Add(kic.Key.Name, lstError);
                }
            }
            return dicError.Count == 0;
        }

        /// <summary>
        ///     查找对象属性值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static T GetValue<TInfo, T>(this TInfo info, string propertyName, T defValue = default(T)) where TInfo : class, new()
        {
            if (info == null) { return defValue; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanRead) { return defValue; }
                return property.GetValue(info, null).ConvertType(defValue);
            }
            return defValue;
        }

        /// <summary>
        ///     设置对象属性值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static void SetValue<TInfo>(this TInfo info, string propertyName, object objValue) where TInfo : class, new()
        {
            if (info == null) { return; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanWrite) { return; }
                property.SetValue(info, objValue.ConvertType(property.PropertyType), null);
            }
        }

        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <typeparam name="TInfo">实体</typeparam>
        public static TInfo TestData<TInfo>(this TInfo info) where TInfo : class, new()
        {
            try
            {
                info = new TInfo();
                Mapping map = typeof(TInfo);
                foreach (var item in map.ModelList)
                {
                    object val;
                    // 对   List 类型处理
                    var argumType = item.Key.PropertyType;
                    if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof(Nullable<>)) { argumType = argumType.GetGenericArguments()[0]; }
                    switch (Type.GetTypeCode(argumType))
                    {
                        case TypeCode.Boolean: val = Rand.GetRandom(0, 1) == 0; break;
                        case TypeCode.DateTime: val = new DateTime(Rand.GetRandom(2000, DateTime.Now.Year), Rand.GetRandom(1, 12), Rand.GetRandom(1, 30), Rand.GetRandom(0, 23), Rand.GetRandom(0, 59), Rand.GetRandom(0, 59)); break;
                        case TypeCode.Char:
                        case TypeCode.Single:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Byte:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64: val = Rand.GetRandom(255); break;
                        default: val = "测试数据"; break;
                    }
                    item.Key.SetValue(info, val.ConvertType(item.Key.PropertyType), null);
                }
                return info;
            }
            catch
            {
                return info.TestData();
            }
        }

        //public static int Insert<TInfo>(this TInfo info) where TInfo : BaseModel<TInfo>, new()
        //{
        //    TInfo.Data.Add(this);
        //    return DbContext.SaveChanges();
        //}

        //public static int Insert<TInfo>(this TInfo info) where TInfo : BaseModel<TInfo>, new()
        //{
        //    Data.Attach(this);
        //    DbContext.Entry<TInfo>(this).State = EntityState.Modified;
        //    return DbContext.SaveChanges();
        //}
    }
}