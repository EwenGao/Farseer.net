using System.Web;
using System.Web.SessionState;
using FS.Configs;
using FS.Extend;

namespace FS.Utils.Web
{
    /// <summary>
    ///     Session工具
    /// </summary>
    public abstract class Sessions : IRequiresSessionState
    {
        /// <summary>
        ///     从 Session 读取 键为 name 的值
        /// </summary>
        /// <param name="name">SessionID</param>
        public static string Get(string name, bool isAddPrefix = true)
        {
            return Get(name, string.Empty, isAddPrefix);
        }

        /// <summary>
        ///     从 Session 读取 键为 name 的值
        /// </summary>
        /// <param name="strName">SessionID</param>
        /// <param name="defValue">为空时返回的值</param>
        public static T Get<T>(string strName, T defValue, bool isAddPrefix = true)
        {
            if (isAddPrefix) { strName = SystemConfigs.ConfigInfo.Session_Prefix + strName; }
            var t = defValue;
            var obj = HttpContext.Current.Session[strName];
            return obj.ConvertType(defValue);
        }

        /// <summary>
        ///     向 Session 保存 键为 name 的， 值为 value
        /// </summary>
        /// <param name="name">SessionID</param>
        /// <param name="value">插入的值</param>
        public static void Set(string strName, object value, bool isAddPrefix = true)
        {
            if (isAddPrefix) { strName = SystemConfigs.ConfigInfo.Session_Prefix + strName; }
            if (value == null){value = string.Empty; }
            HttpContext.Current.Session.Timeout = SystemConfigs.ConfigInfo.Session_TimeOut;
            HttpContext.Current.Session.Add(strName, value);
        }

        /// <summary>
        ///     从 Session 删除 键为 name session 项
        /// </summary>
        /// <param name="name">SessionID</param>
        public static void Remove(string strName, bool isAddPrefix = true)
        {
            if (isAddPrefix) { strName = SystemConfigs.ConfigInfo.Session_Prefix + strName; }
            if (HttpContext.Current.Session[strName] != null)
            {
                HttpContext.Current.Session.Remove(strName);
            }
        }

        /// <summary>
        ///     删除所有 session 项
        /// </summary>
        public static void RemoveAll()
        {
            HttpContext.Current.Session.RemoveAll();
        }
    }
}