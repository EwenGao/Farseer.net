using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace FS.Core.Info
{
    /// <summary>
    /// 所有实体的基类接口
    /// </summary>
    public interface IInfo { }

    /// <summary>
    /// 基类
    /// </summary>
    [Serializable]
    public class BaseInfo<Type> : IInfo, ICloneable
    {
        /// <summary>
        ///     系统编号
        /// </summary>
        [Display(Name = "系统编号")]
        [Column(IsPrimaryKey = true)]
        public virtual Type ID { get; set; }

        /// <summary>
        ///     克隆出一个新的对像
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     克隆出一个新的对像
        /// </summary>
        /// <typeparam name="T">对像，必需继续于IModel</typeparam>
        public T Clone<T>() where T : BaseInfo<Type>
        {
            return MemberwiseClone() as T;
        }
    }

    /// <summary>
    /// 基类
    /// </summary>
    public class BaseInfo : BaseInfo<int?> { }
}