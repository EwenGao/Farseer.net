using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FS.Mapping.Verify
{
    /// <summary>
    ///     ORM 映射关系
    /// </summary>
    public class VerifyMap
    {
        /// <summary>
        ///     获取所有属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, FieldMapState> ModelList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public VerifyMap(Type type)
        {
            ModelList = new Dictionary<PropertyInfo, FieldMapState>();
            object[] attrs;

            //变量属性
            FieldMapState modelAtt;

            #region 变量属性

            //遍历所有属性变量,取得对应使用标记名称
            //无加标记时，则为不使用该变量。
            foreach (var propertyInfo in type.GetProperties())
            {
                modelAtt = new FieldMapState();

                // 是否带属性
                attrs = propertyInfo.GetCustomAttributes(false);
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
                    // 正则
                    if (item is RegularExpressionAttribute) { modelAtt.RegularExpression = (RegularExpressionAttribute)item; continue; }
                }
                if (modelAtt.Display == null) { modelAtt.Display = new DisplayAttribute { Name = propertyInfo.Name }; }
                if (string.IsNullOrEmpty(modelAtt.Display.Name)) { modelAtt.Display.Name = propertyInfo.Name; }

                #region 加入智能错误显示消息

                // 是否必填
                if (modelAtt.Required != null && string.IsNullOrEmpty(modelAtt.Required.ErrorMessage))
                {
                    modelAtt.Required.ErrorMessage = string.Format("{0}，不能为空！", modelAtt.Display.Name);
                }

                // 字符串长度判断
                if (modelAtt.StringLength != null && string.IsNullOrEmpty(modelAtt.StringLength.ErrorMessage))
                {
                    if (modelAtt.StringLength.MinimumLength > 0 && modelAtt.StringLength.MaximumLength > 0)
                    {
                        modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度范围必须为：{1} - {2} 个字符之间！", modelAtt.Display.Name, modelAtt.StringLength.MinimumLength, modelAtt.StringLength.MaximumLength);
                    }
                    else if (modelAtt.StringLength.MaximumLength > 0)
                    {
                        modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度不能大于{1}个字符！", modelAtt.Display.Name, modelAtt.StringLength.MaximumLength);
                    }
                    else
                    {
                        modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度不能小于{1}个字符！", modelAtt.Display.Name, modelAtt.StringLength.MinimumLength);
                    }
                }

                // 值的长度
                if (modelAtt.Range != null && string.IsNullOrEmpty(modelAtt.Range.ErrorMessage))
                {
                    decimal minnum; decimal.TryParse(modelAtt.Range.Minimum.ToString(), out minnum);
                    decimal maximum; decimal.TryParse(modelAtt.Range.Minimum.ToString(), out maximum);

                    if (minnum > 0 && maximum > 0)
                    {
                        modelAtt.Range.ErrorMessage = string.Format("{0}，的值范围必须为：{1} - {2} 之间！", modelAtt.Display.Name, minnum, maximum);
                    }
                    else if (maximum > 0)
                    {
                        modelAtt.Range.ErrorMessage = string.Format("{0}，的值不能大于{1}！", modelAtt.Display.Name, maximum);
                    }
                    else
                    {
                        modelAtt.Range.ErrorMessage = string.Format("{0}，的值不能小于{1}！", modelAtt.Display.Name, minnum);
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
        ///     自增ID
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        private Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator VerifyMap(Type type)
        {
            return VerifyMapCache.GetMap(type);
        }
    }
}