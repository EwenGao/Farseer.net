using System;

namespace FS.Interface
{
    /// <summary>
    ///     审核人信息
    /// </summary>
    public interface IAuditInfo
    {
        /// <summary>
        ///     审核人ID
        /// </summary>
        int? AuditID { get; set; }

        /// <summary>
        ///     审核时间
        /// </summary>
        DateTime? AuditAt { get; set; }

        /// <summary>
        ///     审核人帐号
        /// </summary>
        string AuditName { get; set; }

        /// <summary>
        ///     审核人IP
        /// </summary>
        string AuditIP { get; set; }
    }
}