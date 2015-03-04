using System.ComponentModel.DataAnnotations;

namespace FS.Core.Info
{
    /// <summary>
    /// 分类基类
    /// </summary>
    public class BaseCateInfo<TType> : BaseInfo<TType>
    {
        /// <summary>
        ///     所属ID
        /// </summary>
        [Display(Name = "所属分类")]
        public virtual TType ParentID { get; set; }

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
    public class BaseCateInfo : BaseInfo<int?> { }
}