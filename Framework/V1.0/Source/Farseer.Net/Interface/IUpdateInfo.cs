using System;

namespace FS.Interface
{
    /// <summary>
    ///     修改人信息
    /// </summary>
    public interface IUpdateInfo
    {
        /// <summary>
        ///     修改人ID
        /// </summary>
        int? UpdateID { get; set; }

        /// <summary>
        ///     修改时间
        /// </summary>
        DateTime? UpdateAt { get; set; }

        /// <summary>
        ///     修改人帐号
        /// </summary>
        string UpdateName { get; set; }

        /// <summary>
        ///     修改人IP
        /// </summary>
        string UpdateIP { get; set; }
    }
}