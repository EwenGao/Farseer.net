using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.ORM
{
    /// <summary>
    ///     变量的扩展属性
    /// </summary>
    public class ModelAttribute
    {
        /// <summary>
        ///     字符串长度
        /// </summary>
        public StringLengthAttribute StringLength { get; set; }

        /// <summary>
        ///     是否必填
        /// </summary>
        public RequiredAttribute Required { get; set; }

        /// <summary>
        ///     值描述（字段名）
        /// </summary>
        public DisplayAttribute Display { get; set; }

        /// <summary>
        ///     值的长度
        /// </summary>
        public RangeAttribute Range { get; set; }

        /// <summary>
        ///     数据类型
        /// </summary>
        public DataTypeAttribute DataType { get; set; }

        /// <summary>
        ///     正则
        /// </summary>
        public RegularExpressionAttribute RegularExpression { get; set; }

        /// <summary>
        ///     字段映射
        /// </summary>
        public ColumnAttribute Column { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public DatabaseGeneratedAttribute DatabaseGenerated { get; set; }

        /// <summary>
        ///     主键
        /// </summary>
        public KeyAttribute Key { get; set; }

        /// <summary>
        ///     是否为ORM属性
        /// </summary>
        public bool IsDbField { get; set; }

        /// <summary>
        ///     是否为Json类型
        /// </summary>
        public bool IsJson { get; set; }

        /// <summary>
        ///     扩展类型
        /// </summary>
        public eumPropertyExtend PropertyExtend { get; set; }
    }

    /// <summary>
    ///     设置为Json变量类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IsJsonAttribute : Attribute
    {
        /// <summary>
        ///     设置为Json变量类型
        /// </summary>
        public IsJsonAttribute()
        {
            IsJson = true;
        }

        /// <summary>
        ///     设置为Json变量类型
        /// </summary>
        internal bool IsJson { get; set; }
    }

    /// <summary>
    ///     设置变量的扩展属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyExtendAttribute : Attribute
    {
        /// <summary>
        ///     设置变量的扩展属性
        /// </summary>
        public PropertyExtendAttribute(eumPropertyExtend propertyExtend)
        {
            PropertyExtend = propertyExtend;
        }

        /// <summary>
        ///     设置变量的扩展属性
        /// </summary>
        internal eumPropertyExtend PropertyExtend { get; set; }
    }

    /// <summary>
    ///     属性类型，自定义扩展属性
    /// </summary>
    public enum eumPropertyExtend
    {
        /// <summary>
        ///     Xml属性
        /// </summary>
        Attribute,

        /// <summary>
        ///     Xml节点
        /// </summary>
        Element,
    }
}