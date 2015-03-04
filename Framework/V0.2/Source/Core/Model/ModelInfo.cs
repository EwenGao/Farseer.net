using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using FS.Mapping.Verify;

namespace FS.Core.Model
{
    /// <summary>
    /// 基类
    /// </summary>
    [Serializable]
    public class ModelInfo : ICloneable, IVerification
    {
        /// <summary>
        ///     系统编号
        /// </summary>
        [Display(Name = "系统编号")]
        [Column(IsDbGenerated = true)]
        public virtual int? ID { get; set; }

        /// <summary>
        ///     克隆出一个新的对象
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     克隆出一个新的对象
        /// </summary>
        /// <typeparam name="T">对象，必须继续于IModel</typeparam>
        public T Clone<T>() where T : ModelInfo
        {
            return MemberwiseClone() as T;
        }
    }
}