using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Demo.Common
{
    /// <summary>
    /// 用户组
    /// </summary>
    public enum eumGenderType : byte
    {
        /// <summary>
        /// 男士
        /// </summary>
        [Display(Name = "男士")]
        Man = 0,

        /// <summary>
        /// 女士
        /// </summary>
        [Display(Name = "女士")]
        Woman
    }
}
