using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using FS.Core.Data;
using FS.Core.Model;
using FS.Mapping.Table;
using FS.Mapping.Verify;
using FS.Utils.Common;
using FS.Utils.Web;

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
        public static void Fill<TInfo>(this TInfo info, Page page, string contentPlaceHolderID, string prefix = "hl")
            where TInfo : ModelInfo, new()
        {
            if (info == null)
            {
                return;
            }

            var masterControl = page.FindControl(contentPlaceHolderID);
            if (masterControl == null)
            {
                return;
            }

            Fill(masterControl.Controls, info, prefix);
        }

        /// <summary>
        ///     将实体类填充到控件中
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="prefix">控件前缀</param>
        /// <param name="info">要填充的值</param>
        public static void Fill<TInfo>(this TInfo info, Page page, string prefix = "hl") where TInfo : ModelInfo, new()
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
        private static void Fill<TInfo>(ControlCollection controls, TInfo infoValue, string prefix = "hl") where TInfo : ModelInfo, new()
        {
            if (infoValue == null || controls == null)
            {
                return;
            }
            var map = TableMapCache.GetMap(infoValue);
            foreach (var kic in map.ModelList)
            {
                // 当前成员的值
                var value = kic.Key.GetValue(infoValue, null);
                if (value == null) { continue; }

                var type = value.GetType();

                // 当前成员，是一个类
                if (value is ModelInfo)
                {
                    foreach (var item in type.GetProperties()) { Fill(controls, (ModelInfo)value, prefix); }
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
        public static bool Check<TInfo>(this TInfo info, Action<string, string> tip = null, string url = "") where TInfo : IVerification
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
        public static bool Check<TInfo>(this TInfo info, Action<Dictionary<string, List<string>>> tip) where TInfo : IVerification
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
        public static bool Check<TInfo>(this TInfo info, out Dictionary<string, List<string>> dicError) where TInfo : IVerification
        {
            dicError = new Dictionary<string, List<string>>();
            var map = VerifyMapCache.GetMap(info);
            foreach (var kic in map.ModelList)
            {
                var lstError = new List<string>();
                var value = kic.Key.GetValue(info, null);

                // 是否必填
                if (kic.Value.Required != null && !kic.Value.Required.IsValid(value))
                {
                    lstError.Add(kic.Value.Required.ErrorMessage);
                    //dicError.Add(kic.Key.Name, lstError);
                }

                //if (value == null) { continue; }

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
        public static T GetValue<TInfo, T>(this TInfo info, string propertyName, T defValue = default(T)) where TInfo : ModelInfo, new()
        {
            if (info == null) { return defValue; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanRead) { return defValue; }
                return property.GetValue(info, null).ConvertType(defValue);
            }
            return defValue;
            //if (info == null) { return defValue; }
            //Mapping Map = typeof(TInfo);
            //var lstModel = Map.ModelList.Where(o => o.Key.Name.IsEquals(propertyName));
            //return lstModel.Count() == 0 ? defValue : lstModel.FirstOrDefault().Key.GetValue(info, null).ConvertType(defValue);
        }

        /// <summary>
        ///     设置对象属性值
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="info">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static void SetValue<TInfo>(this TInfo info, string propertyName, object objValue) where TInfo : ModelInfo, new()
        {
            if (info == null) { return; }
            foreach (var property in info.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanWrite) { return; }
                property.SetValue(info, objValue.ConvertType(property.PropertyType), null);
            }

            //if (info == null) { return; }
            //Mapping Map = typeof(TInfo);
            //var lstModel = Map.ModelList.Where(o => o.Key.Name.IsEquals(propertyName));
            //if (lstModel.Count() == 0) { return; }
            //lstModel.FirstOrDefault().Key.SetValue(info, objValue, null);
        }

        /// <summary>
        ///     关联两个实体
        /// </summary>
        /// <typeparam name="T1">主实体</typeparam>
        /// <typeparam name="T2">要附加关联的实体</typeparam>
        /// <param name="info">主数据</param>
        /// <param name="JoinModule">要关联的子实体</param>
        /// <param name="JoinModuleSelect">要附加关联的子实体的字段筛选</param>
        /// <param name="JoinModuleID">主表关系字段</param>
        /// <param name="defJoinModule">为空时如何处理？</param>
        /// <param name="db">事务</param>
        public static T1 Join<T1, T2>(this T1 info, Expression<Func<T1, T2>> JoinModule, Func<T1, int?> JoinModuleID = null, Expression<Func<T2, object>> JoinModuleSelect = null, T2 defJoinModule = null, DbExecutor db = null)
            where T1 : ModelInfo, new()
            where T2 : ModelInfo, new()
        {
            if (info == null)
            {
                return null;
            }
            if (JoinModuleID == null)
            {
                JoinModuleID = o => o.ID;
            }

            #region 获取实际类型

            var memberExpression = JoinModule.Body as MemberExpression;
            // 获取属性类型
            var propertyType = (PropertyInfo)memberExpression.Member;

            var lstPropery = new List<PropertyInfo>();
            while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = memberExpression.Expression as MemberExpression;
                lstPropery.Add((PropertyInfo)memberExpression.Member);
            }
            lstPropery.Reverse();

            #endregion

            // 详细资料
            var subInfo = (new T2()) is BaseCacheModel<T2>
                             ? BaseCacheModel<T2>.Cache(db).Where(o => o.ID == JoinModuleID.Invoke(info)).ToInfo()
                             : BaseModel<T2>.Data.Where(o => o.ID == JoinModuleID.Invoke(info))
                                            .Select(JoinModuleSelect)
                                            .ToInfo(db) ?? defJoinModule;

            object value = info;
            foreach (var propery in lstPropery)
            {
                value = propery.GetValue(value, null);
            }

            propertyType.SetValue(value, subInfo, null);

            return info;
        }

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public static bool Insert<TInfo>(this TInfo info, DbExecutor db = null) where TInfo : ModelInfo, new()
        {
            if (info is BaseCacheModel<TInfo>) { return BaseCacheModel<TInfo>.Data.Insert(info, db); }
            return BaseModel<TInfo>.Data.Insert(info, db);
        }

        /// <summary>
        ///     插入数据
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="identity">标识，刚插入的ID</param>
        public static bool Insert<TInfo>(this TInfo info, out int identity, DbExecutor db = null) where TInfo : ModelInfo, new()
        {
            if ((new TInfo()) is BaseCacheModel<TInfo>) { return BaseCacheModel<TInfo>.Data.Insert(info, out identity, db); }
            return BaseModel<TInfo>.Data.Insert(info, out identity, db);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        public static bool Update<TInfo>(this TInfo info, DbExecutor db = null) where TInfo : ModelInfo, new()
        {
            if ((new TInfo()) is BaseCacheModel<TInfo>) { return BaseCacheModel<TInfo>.Data.Update(info, db); }
            return BaseModel<TInfo>.Data.Update(info, db);
        }

        /// <summary>
        ///     更新数据
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="db">可传入事务的db</param>
        public static bool Update<TInfo>(this TInfo info, Expression<Func<TInfo, bool>> where, DbExecutor db = null) where TInfo : ModelInfo, new()
        {
            if ((new TInfo()) is BaseCacheModel<TInfo>) { return BaseCacheModel<TInfo>.Data.Where(where).Update(info, db); }
            return BaseModel<TInfo>.Data.Where(where).Update(info, db);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        public static bool Update<TInfo>(this TInfo info, int? ID, DbExecutor db = null) where TInfo : ModelInfo, new()
        {
            if ((new TInfo()) is BaseCacheModel<TInfo>) { return BaseCacheModel<TInfo>.Data.Where(o => o.ID == ID).Update(info, db); }
            return BaseModel<TInfo>.Data.Where(o => o.ID == ID).Update(info, db);
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="db">可传入事务的db</param>
        /// <param name="IDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        public static bool Update<TInfo>(this TInfo info, List<int> IDs, DbExecutor db = null) where TInfo : ModelInfo, new()
        {
            if ((new TInfo()) is BaseCacheModel<TInfo>) { return BaseCacheModel<TInfo>.Data.Where(o => IDs.Contains(o.ID)).Update(info, db); }
            return BaseModel<TInfo>.Data.Where(o => IDs.Contains(o.ID)).Update(info, db);
        }

        /// <summary>
        ///     保存数据
        /// </summary>
        /// <param name="reqID">请求ID</param>
        /// <param name="tip">提示</param>
        /// <param name="actInsert">插入时的方法委托</param>
        /// <param name="actUpdate">更新时的方法委托</param>
        /// <param name="actSuccess">成功后的方法委托</param>
        public static void Save<TInfo>(this TInfo info, int reqID, Action<string, string> tip = null, Action<TInfo, DbExecutor> actInsert = null, Action<TInfo, DbExecutor> actUpdate = null, Action<int, TInfo, DbExecutor> actSuccess = null) where TInfo : ModelInfo, new()
        {
            if (!info.Check(tip)) { return; }

            using (DbExecutor db = typeof(TInfo))
            {
                if (reqID > 0) { if (actUpdate != null) { actUpdate(info, db); } info.Update(reqID, db); }
                else { if (actInsert != null) { actInsert(info, db); } info.Insert(out reqID, db); }
                if (actSuccess != null) { actSuccess(reqID, info, db); }
                db.Commit();
            }
        }

        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <typeparam name="TInfo">实体</typeparam>
        /// <param name="info">任意对象</param>
        /// <param name="subCount">如果成员包含List类型时，要填充的数量</param>
        public static TInfo TestData<TInfo>(this TInfo info, int subCount = 10)
        {
            return TestData(info, 1, subCount);
        }

        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <typeparam name="TInfo">实体</typeparam>
        /// <param name="subCount">如果成员包含List类型时，要填充的数量</param>
        /// <param name="level">防止无限层</param>
        private static TInfo TestData<TInfo>(this TInfo info, int level, int subCount)
        {
            var type = info.GetType();
            foreach (var item in type.GetProperties())
            {
                if (!item.CanWrite) { continue; }

                var argumType = item.PropertyType;
                // 去掉Nullable的类型
                if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof(Nullable<>)) { argumType = argumType.GetGenericArguments()[0]; }

                try
                {
                    // 对   List 类型处理
                    if (item.PropertyType.IsGenericType)
                    {
                        // 动态构造List
                        var objLst = Activator.CreateInstance(item.PropertyType);
                        // List元素
                        var objItem = Activator.CreateInstance(argumType.GetGenericArguments()[0]);

                        // 防止无限层递归
                        if (level < 3)
                        {
                            for (var i = 0; i < subCount; i++)
                            {
                                item.PropertyType.GetMethod("Add").Invoke(objLst, new[] {TestData(objItem, level + 1, subCount)});
                            }
                        }
                        item.SetValue(info, objLst, null);
                        continue;
                    }
                    // 普通成员
                    item.SetValue(info, TestData(argumType), null);
                }
                catch
                {

                }
            }
            return info;
        }


        /// <summary>
        /// 返回基本类型的数据
        /// </summary>
        /// <param name="argumType"></param>
        /// <returns></returns>
        public static object TestData(Type argumType)
        {
            var val = new object();
            // 对   List 类型处理
            if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return TestData(argumType.GetGenericArguments()[0]); }
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
            return val.ConvertType(argumType);
        }
    }
}