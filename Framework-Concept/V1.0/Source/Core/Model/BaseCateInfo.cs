using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Core.Model
{
    /// <summary>
    /// 分类基类
    /// </summary>
    /// <typeparam name="Type">主键类型</typeparam>
    public class BaseCateInfo<Type> : BaseInfo<Type>
    {
        /// <summary>
        ///     所属ID
        /// </summary>
        [Display(Name = "所属分类")]
        public virtual Type ParentID { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        [Display(Name = "标题"), StringLength(50), Required]
        public virtual string Caption { get; set; }

        /// <summary>
        ///     排序
        /// </summary>
        [Display(Name = "排序")]
        public virtual int? Sort { get; set; }
    }

    /// <summary>
    /// 分类基类
    /// </summary>
    public class BaseCateInfo : BaseCateInfo<int> { }
}
