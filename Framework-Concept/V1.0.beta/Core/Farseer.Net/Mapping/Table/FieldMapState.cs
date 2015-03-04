using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace FS.Mapping.Table
{
    /// <summary>
    /// 保存字段映射的信息
    /// </summary>
    public class FieldMapState
    {
        public FieldMapState()
        {
            IsDbField = true;
        }

        /// <summary>
        ///     数据类型
        /// </summary>
        public DataTypeAttribute DataType { get; set; }

        /// <summary>
        ///     字段映射
        /// </summary>
        public ColumnAttribute Column { get; set; }

        /// <summary>
        ///     是否为ORM属性
        /// </summary>
        public bool IsDbField { get; set; }

        /// <summary>
        ///     扩展类型
        /// </summary>
        public eumPropertyExtend PropertyExtend { get; set; }
    }

    /// <summary>
    ///     设置为非字段映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotJoinAttribute : Attribute { }

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