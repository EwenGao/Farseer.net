using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Core.Model
{
    /// <summary>
    /// 基类
    /// </summary>
    /// <typeparam name="Type">主键类型</typeparam>
    public class BaseInfo<Type> : ICloneable 
    {
        /// <summary>
        ///     系统编号
        /// </summary>
        [Display(Name = "系统编号")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Type ID { get; set; }

        /// <summary>
        ///  克隆出一个新的对像
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///  克隆出一个新的对像
        /// </summary>
        /// <typeparam name="T">对像，必需继续于BaseModel</typeparam>
        public TInfo Clone<TInfo>() where TInfo : BaseInfo<Type>
        {
            return MemberwiseClone() as TInfo;
        }
    }

    /// <summary>
    /// 基类
    /// </summary>
    public class BaseInfo : BaseInfo<int> { }
}
