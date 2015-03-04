using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FS.Extend;
using FS.Utils.Common;

namespace FS.Utils.Open
{
    /// <summary>
    /// QQ开放平台组件
    /// 如何使用：
    /// 第一步：调用静态方法  GetAuthorizeUrl 返回跳转地址。
    /// 第二步：前台进行GET方式跳转得到的URL。（将会去到QQ的登陆界面。）
    /// 第三步：当用户点击登陆后，将返回，你传入的回调页面。在回调页面中，实例化本对象
    /// 第四步：调用 GetOpenID 获取OpenID。调用 GetUserInfo 获取用户的基本信息
    /// </summary>
    public class QQWebOpen
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; private set; }
        /// <summary>
        /// 应用密码
        /// </summary>
        public string AppKey { get; private set; }
        /// <summary>
        /// 授权码
        /// </summary>
        public string AuthorizationCode { get; private set; }
        /// <summary>
        /// 授权令牌
        /// </summary>
        public string AccessToken { get; private set; }
        /// <summary>
        /// 该access token的有效期，单位为秒。
        /// </summary>
        public string ExpiresIn { get; private set; }
        /// <summary>
        /// 在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// OpenID
        /// </summary>
        public string OpenID { get; private set; }

        /// <summary>
        /// 必须传入应用的ID跟密码
        /// </summary>
        /// <param name="appID">应用ID</param>
        /// <param name="appKey">应用密码</param>
        /// <param name="authorizationCode">授权码</param>
        /// <param name="redirectUri">成功授权后的回调地址，必须是注册appid时填写的主域名下的地址，建议设置为网站首页或网站的用户中心。注意需要将url进行URLEncode。</param>
        public QQWebOpen(string appID, string appKey, string authorizationCode, string redirectUri)
        {
            AppID = appID;
            AppKey = appKey;
            AuthorizationCode = authorizationCode;

            // 通过授权码、请求获取令牌 
            var url = string.Format("https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri={3}", AppID, AppKey, AuthorizationCode, redirectUri);
            var json = Net.Get(url);

            // 封装参数
            AccessToken = Web.Url.GetParm(json, "access_token");   //  授权令牌
            ExpiresIn = Web.Url.GetParm(json, "expires_in");     //  该access token的有效期，单位为秒。
            RefreshToken = Web.Url.GetParm(json, "refresh_token");   //  在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
        }

        /// <summary>
        /// 获取OpenID
        /// </summary>
        /// <returns></returns>
        public string GetOpenID()
        {
            if (OpenID.IsHaving()) { return OpenID; }

            // 通过令牌，请求获取openid（用户身份）
            var url = string.Format("https://graph.qq.com/oauth2.0/me?access_token={0}", AccessToken);
            var json = Net.Get(url);
            OpenID = Regex.Match(json, @"(?<=""openid"":"")[^""]*(?="")").Value;
            return OpenID;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            return API("get_user_info", "");
        }

        /// <summary>
        /// 与开放平台通讯的通用接口
        /// </summary>
        /// <param name="apiName">接口名称</param>
        /// <param name="paras">接口的参数，不需要传入：令牌、AppID、OpenID</param>
        /// <param name="method">GET/POST</param>
        public string API(string apiName, string paras, string method = "GET")
        {
            var url = string.Format("https://graph.qq.com/user/{0}", apiName);
            var param = string.Format("?access_token={0}&oauth_consumer_key={1}&openid={2}", AccessToken, AppID, GetOpenID(), paras);
            if (method.ToUpper() == "GET") { return Net.Get(url + param); }

            return Net.Post(url, param);

        }

        /// <summary>
        /// 获取授权码（返回Url，手动进行Get跳转）
        /// </summary>
        public static string GetAuthorizeUrl(string appID, string callBackUrl, string state)
        {
            return string.Format("https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={2}", appID, callBackUrl, state);
        }
    }
}
