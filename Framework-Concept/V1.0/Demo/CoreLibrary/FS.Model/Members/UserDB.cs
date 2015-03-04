using FS.Core.Bean;
using FS.Core.Model;
using FS.ORM;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace FS.Model.Members
{
    [DB("Members_User")]
    public class UserDB : BaseModel<UserDB>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [StringLength(50)]
        [Required()]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [StringLength(50)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string PassWord { get; set; }

        /// <summary>
        /// 会员类型
        /// </summary>
        [Display(Name = "会员类型")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public byte GenderType { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")]
        [StringLength(50)]
        //[Required()]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Email { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        [Display(Name = "登陆次数")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int LoginCount { get; set; }

        /// <summary>
        /// 登陆IP
        /// </summary>
        [Display(Name = "登陆IP")]
        [StringLength(15)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string LoginIP { get; set; }

        /// <summary>
        /// 登陆时间
        /// </summary>
        [Display(Name = "登陆时间")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime LoginAt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// 用户组
        /// </summary>
        public enum eumGenderType : int
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
