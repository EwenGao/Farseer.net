using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS.Core.Model;
using FS.ORM;

namespace FS.Model
{
    [DB(Name = "Speed")]
    public class SpeedDB : BaseModel<SpeedDB>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(50)]
        [Required()]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [StringLength(50)]
        public string PassWord { get; set; }

        /// <summary>
        /// 会员类型
        /// </summary>
        [Display(Name = "会员类型")]
        public eumGenderType? GenderType { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        [Display(Name = "登陆次数")]
        public int? LoginCount { get; set; }

        /// <summary>
        /// 登陆IP
        /// </summary>
        [Display(Name = "登陆IP")]
        [StringLength(15)]
        public string LoginIP { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        public int? RoleID { get; set; }

        public DateTime? CreateAt { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public enum eumGenderType
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
}
