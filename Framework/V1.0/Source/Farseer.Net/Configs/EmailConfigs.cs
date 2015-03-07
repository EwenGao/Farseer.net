using System;
using System.Collections.Generic;

namespace FS.Configs
{
    /// <summary>
    ///     全局
    /// </summary>
    public class EmailConfigs : BaseConfigs<EmailConfig> { }

    /// <summary>
    ///     Email配置信息类
    /// </summary>
    [Serializable]
    public class EmailConfig
    {
        /// <summary>
        ///     Email配置,从/App_Data/Db.Configs读取回来
        /// </summary>
        public List<EmailInfo> EmailList = new List<EmailInfo>();
    }

    /// <summary>
    ///     E-mail配置
    /// </summary>
    public class EmailInfo
    {
        /// <summary>
        ///     是否Html邮件
        /// </summary>
        public bool IsHtml = false;

        /// <summary>
        ///     登陆用户名
        /// </summary>
        public string LoginName = "";

        /// <summary>
        ///     登陆密码
        /// </summary>
        public string LoginPwd = "";

        /// <summary>
        ///     最多收件人数量
        /// </summary>
        public int RecipientMaxNum = 5;

        /// <summary>
        ///     发件人E-Mail地址
        /// </summary>
        public string SendMail = "";

        /// <summary>
        ///     发件人姓名
        /// </summary>
        public string SendUserName = "";

        /// <summary>
        ///     端口号
        /// </summary>
        public int SmtpPort = 25;

        /// <summary>
        ///     邮件服务器域名和验证信息
        ///     形如：Smtp.server.com"
        /// </summary>
        public string SmtpServer = "";

        /// <summary>
        ///     通过索引返回实体
        /// </summary>
        public static implicit operator EmailInfo(int index)
        {
            return EmailConfigs.ConfigInfo.EmailList.Count <= index ? null : EmailConfigs.ConfigInfo.EmailList[index];
        }
    }
}