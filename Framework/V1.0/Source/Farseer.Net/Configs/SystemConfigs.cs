using System;

namespace FS.Configs
{
    /// <summary>
    ///     系统配置
    /// </summary>
    public class SystemConfigs : BaseConfigs<SystemConfig> { }

    /// <summary>
    ///     配置文件
    /// </summary>
    [Serializable]
    public class SystemConfig
    {
        /// <summary>
        ///     缓存逻辑超时时间(分钟)
        /// </summary>
        public int Cache_Db_TimeOut = 1440;

        /// <summary>
        ///     缓存片断超时时间(分钟)
        /// </summary>
        public int Cache_List_TimeOut = 120;

        /// <summary>
        ///     管理员登陆验证码的
        /// </summary>
        public string Cookies_Admin_VerifyCode = "Cookies_Admin_VerifyCode";

        /// <summary>
        ///     Cookies前缀
        /// </summary>
        public string Cookies_Prefix = "QynV5.0";

        /// <summary>
        ///     Cookies超时时间(分钟)
        /// </summary>
        public int Cookies_TimeOut = 20;

        /// <summary>
        ///     用户名
        /// </summary>
        public string Cookies_User_Name = "Cookies_User_Name";

        /// <summary>
        ///     用户登陆验证码的
        /// </summary>
        public string Cookies_User_VerifyCode = "Cookies_User_VerifyCode";

        /// <summary>
        ///     会员类型
        /// </summary>
        public string Cookies_User_Role = "Cookies_User_Role";

        /// <summary>
        /// 记住最后访问地址
        /// </summary>
        public string Cookies_CallBack_Url = "Cookies_CallBack_Url";

        /// <summary>
        ///     开启记录数据库执行过程
        /// </summary>
        public bool IsWriteDbLog = false;

        /// <summary>
        ///     管理员ID的Cookies
        /// </summary>
        public string Session_Admin = "Session_Admin";

        /// <summary>
        ///     管理员ID的Cookies
        /// </summary>
        public string Session_User = "Session_User";

        /// <summary>
        ///     用户ID
        /// </summary>
        public string Session_User_ID = "Session_User_ID";

        /// <summary>
        ///     Session前缀
        /// </summary>
        public string Session_Prefix = "QynV5.0";

        /// <summary>
        ///     Session超时时间(分钟)
        /// </summary>
        public int Session_TimeOut = 20;
    }
}