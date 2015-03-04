using FS.Configs;
using FS.Core.Bean;
using FS.Core.Data;
using FS.Extend;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace FS.ORM
{
    /// <summary>
    ///     ORM 映射关系
    /// </summary>
    public class Mapping
    {
        /// <summary>
        ///     获取所有属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, ModelAttribute> ModelList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        internal Mapping(Type type)
        {
            ModelList = new Dictionary<PropertyInfo, ModelAttribute>();
            object[] attrs;

            //变量属性
            ModelAttribute modelAtt;

            #region 类属性

            //类属性
            attrs = type.GetCustomAttributes(typeof(DBAttribute), false);
            ClassInfo = attrs.Length == 0 ? new DBAttribute(type.Name) : ((DBAttribute)attrs[0]);

            #region 自动创建数据库配置文件

            if (DbConfigs.ConfigInfo.DbList.Count == 0 && ClassInfo.DbIndex == 0)
            {
                var db = new DbConfig();
                db.DbList.Add(new DbInfo
                                  {
                                      Catalog = "数据库名称",
                                      CommandTimeout = 60,
                                      ConnectTimeout = 30,
                                      DataType = DBType.SqlServer,
                                      DataVer = "2008",
                                      PassWord = "123456",
                                      PoolMaxSize = 100,
                                      PoolMinSize = 16,
                                      Server = ".",
                                      UserID = "sa"
                                  });
                DbConfigs.SaveConfig(db);
            }
            else if (DbConfigs.ConfigInfo.DbList.Count - 1 < ClassInfo.DbIndex)
            {
                throw new Exception("数据库配置(索引项：" + ClassInfo.DbIndex + ")不存在！");
            }

            #endregion

            #region 获取DbConfig的配置

            DbInfo dbInfo = ClassInfo.DbIndex;
            ClassInfo.ConnStr = DbFactory.CreateConnString(ClassInfo.DbIndex);
            ClassInfo.DataType = dbInfo.DataType;
            ClassInfo.DataVer = dbInfo.DataVer;
            ClassInfo.CommandTimeout = dbInfo.CommandTimeout;

            #endregion

            #endregion

            #region 变量属性

            //遍历所有属性变量,取得对应使用标记名称
            //无加标记时，则为不使用该变量。
            foreach (var propertyInfo in type.GetProperties())
            {
                modelAtt = new ModelAttribute();

                // 是否带属性
                attrs = propertyInfo.GetCustomAttributes(false);
                modelAtt.IsDbField = attrs.Length > 0;
                foreach (var item in attrs)
                {
                    // 字符串长度
                    if (item is StringLengthAttribute) { modelAtt.StringLength = (StringLengthAttribute)item; continue; }
                    // 是否必填
                    if (item is RequiredAttribute) { modelAtt.Required = (RequiredAttribute)item; continue; }
                    // 字段描述
                    if (item is DisplayAttribute) { modelAtt.Display = (DisplayAttribute)item; continue; }
                    // 值的长度
                    if (item is RangeAttribute) { modelAtt.Range = (RangeAttribute)item; continue; }
                    // 数据类型
                    if (item is DataTypeAttribute) { modelAtt.DataType = (DataTypeAttribute)item; continue; }
                    // 正则
                    if (item is RegularExpressionAttribute) { modelAtt.RegularExpression = (RegularExpressionAttribute)item; continue; }
                    // 属性扩展
                    if (item is PropertyExtendAttribute) { modelAtt.PropertyExtend = ((PropertyExtendAttribute)item).PropertyExtend; continue; }
                    // 字段映射
                    if (item is ColumnAttribute) { modelAtt.Column = (ColumnAttribute)item; continue; }
                    // 主键
                    if (item is KeyAttribute) { modelAtt.Key = (KeyAttribute)item; continue; }
                    // 标识
                    if (item is DatabaseGeneratedAttribute) { modelAtt.DatabaseGenerated = (DatabaseGeneratedAttribute)item; continue; }
                    // 加入属性
                    if (item is NotMappedAttribute) { modelAtt.IsDbField = false; continue; }
                    // Json
                    modelAtt.IsJson = item is IsJsonAttribute;
                }
                if (modelAtt.Display == null || modelAtt.Display.Name.IsNullOrEmpty()) { modelAtt.Display = new DisplayAttribute { Name = propertyInfo.Name }; }
                if (modelAtt.Column == null || modelAtt.Column.Name.IsNullOrEmpty()) { modelAtt.Column = new ColumnAttribute(propertyInfo.Name); }
                if (modelAtt.DatabaseGenerated != null && modelAtt.DatabaseGenerated.DatabaseGeneratedOption != DatabaseGeneratedOption.None) { IndexName = modelAtt.Column.Name; }

                #region 加入智能错误显示消息

                // 是否必填
                if (modelAtt.Required != null && modelAtt.Required.ErrorMessage.IsNullOrEmpty())
                {
                    modelAtt.Required.ErrorMessage = string.Format("{0}，不能为空！", modelAtt.Display.Name);
                }

                // 字符串长度判断
                if (modelAtt.StringLength != null && modelAtt.StringLength.ErrorMessage.IsNullOrEmpty())
                {
                    if (modelAtt.StringLength.MinimumLength > 0 && modelAtt.StringLength.MaximumLength > 0)
                    {
                        modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度范围必需为：{1} - {2} 个字符之间！",
                                                                           modelAtt.Display.Name,
                                                                           modelAtt.StringLength.MinimumLength,
                                                                           modelAtt.StringLength.MaximumLength);
                    }
                    else if (modelAtt.StringLength.MaximumLength > 0)
                    {
                        modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度不能大于{1}个字符！", modelAtt.Display.Name,
                                                                           modelAtt.StringLength.MaximumLength);
                    }
                    else
                    {
                        modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度不能小于{1}个字符！", modelAtt.Display.Name,
                                                                           modelAtt.StringLength.MinimumLength);
                    }
                }

                // 值的长度
                if (modelAtt.Range != null && modelAtt.Range.ErrorMessage.IsNullOrEmpty())
                {
                    if (modelAtt.Range.Minimum.ConvertType(0m) > 0 && modelAtt.Range.Maximum.ConvertType(0m) > 0)
                    {
                        modelAtt.Range.ErrorMessage = string.Format("{0}，的值范围必需为：{1} - {2} 之间！", modelAtt.Display.Name,
                                                                    modelAtt.Range.Minimum.ConvertType(0m),
                                                                    modelAtt.Range.Maximum.ConvertType(0m));
                    }
                    else if (modelAtt.Range.Maximum.ConvertType(0m) > 0)
                    {
                        modelAtt.Range.ErrorMessage = string.Format("{0}，的值不能大于{1}！", modelAtt.Display.Name,
                                                                    modelAtt.Range.Maximum.ConvertType(0m));
                    }
                    else
                    {
                        modelAtt.Range.ErrorMessage = string.Format("{0}，的值不能小于{1}！", modelAtt.Display.Name,
                                                                    modelAtt.Range.Minimum.ConvertType(0m));
                    }
                }

                #endregion

                //添加属变量标记名称
                ModelList.Add(propertyInfo, modelAtt);
            }
            #endregion

            Type = type;
        }

        /// <summary>
        ///     类属性
        /// </summary>
        public DBAttribute ClassInfo { get; set; }

        /// <summary>
        ///     索引名称
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator Mapping(Type type)
        {
            return ModelCache.GetInfo(type);
        }

        /// <summary>
        ///     获取当前属性（通过使用的userName）
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public KeyValuePair<PropertyInfo, ModelAttribute> GetModelInfo(string userName = "")
        {
            return userName.IsNullOrEmpty() ? ModelList.FirstOrDefault(oo => oo.Value.Key != null) : ModelList.FirstOrDefault(oo => oo.Key.Name == userName);
        }

        /// <summary>
        ///     获取标注的名称
        /// </summary>
        /// <param name="propertyInfo">属性变量</param>
        /// <returns></returns>
        public string GetUserName(PropertyInfo propertyInfo)
        {
            return ModelList[propertyInfo].Column.Name;
        }
    }
}