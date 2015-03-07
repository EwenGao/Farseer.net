using System;

namespace FS.Interface
{
    /// <summary>
    ///     创建人信息
    /// </summary>
    public interface ICreateInfo
    {
        /// <summary>
        ///     创建人ID
        /// </summary>
        int? CreateID { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        DateTime? CreateAt { get; set; }

        /// <summary>
        ///     创建人帐号
        /// </summary>
        string CreateName { get; set; }

        /// <summary>
        ///     创建人IP
        /// </summary>
        string CreateIP { get; set; }
    }
}