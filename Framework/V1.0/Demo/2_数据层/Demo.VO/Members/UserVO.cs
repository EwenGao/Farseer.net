using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.Common;

namespace Demo.VO.Members
{
    public class UserListVO
    {
        public int ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 会员类型
        /// </summary>
        public eumGenderType GenderType { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        public int LoginCount { get; set; }

        /// <summary>
        /// 登陆IP
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 登陆时间
        /// </summary>
        public DateTime LoginAt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
