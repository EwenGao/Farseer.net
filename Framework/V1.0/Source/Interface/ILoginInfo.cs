using System;

namespace FS.Interface
{
    /// <summary>
    ///     登陆信息
    /// </summary>
    public interface ILoginInfo
    {
        /// <summary>
        ///     登陆次数
        /// </summary>
        int? LoginCount { get; set; }

        /// <summary>
        ///     登陆IP
        /// </summary>
        string LoginIP { get; set; }

        /// <summary>
        ///     登陆时间
        /// </summary>
        DateTime? LoginAt { get; set; }
    }
}