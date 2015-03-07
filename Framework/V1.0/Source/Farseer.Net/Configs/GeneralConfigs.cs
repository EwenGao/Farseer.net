using System;

namespace FS.Configs
{
    /// <summary>
    ///     全局
    /// </summary>
    public class GeneralConfigs : BaseConfigs<GeneralConfig> { }

    /// <summary>
    ///     网站基本设置描述类, 加[Serializable]标记为可序列化
    /// </summary>
    [Serializable]
    public class GeneralConfig
    {
        /// <summary>
        ///     Cookies域，不填，则自动当前域
        /// </summary>
        public string CookiesDomain = "";

        /// <summary>
        ///     是否调试
        /// </summary>
        public bool DeBug = false;

        /// <summary>
        ///     重写域名替换(多个用;分隔)
        /// </summary>
        public string RewriterDomain = "qyn99.com;";

        /// <summary>
        ///     忽略登陆判断地址(多个用;分隔)
        /// </summary>
        public string IgnoreLogin = "/Login.aspx;";

        /// <summary>
        ///     上传文件的目录
        /// </summary>
        public string UploadDirectory = "/UpLoadFile/";

        /// <summary>
        ///     网站标题
        /// </summary>
        public string WebTitle = "感谢使用Farseer.Net V0.2";
    }
}