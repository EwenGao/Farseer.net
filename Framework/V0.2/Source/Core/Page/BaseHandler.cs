using System.Text;
using System.Web.SessionState;
using FS.Utils.Web;

namespace FS.Core.Page
{
    /// <summary>
    /// Handler基类
    /// </summary>
    public class BaseHandler : IRequiresSessionState
    {
        #region Request

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public string QS(string parmsName, Encoding encoding)
        {
            return Req.QS(parmsName, encoding);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public string QS(string parmsName)
        {
            return Req.QS(parmsName);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        public T QS<T>(string parmsName, T defValue)
        {
            return Req.QS(parmsName, defValue);
        }

        /// <summary>
        ///     Request.Form
        /// </summary>
        public T QF<T>(string parmsName, T defValue)
        {
            return Req.QF(parmsName, defValue);
        }

        /// <summary>
        ///     Request.Form
        /// </summary>
        public string QF(string parmsName)
        {
            return Req.QF(parmsName);
        }

        /// <summary>
        ///     先QF后QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        public string QA(string parmsName)
        {
            return Req.QA(parmsName);
        }

        /// <summary>
        ///     先QF后QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        public T QA<T>(string parmsName, T defValue)
        {
            return Req.QA(parmsName, defValue);
        }

        #endregion

        /// <summary>
        ///     转到网址
        /// </summary>
        public void GoToUrl(string url, params object[] args)
        {
            Req.GoToUrl(url, args);
        }

        /// <summary>
        ///     转到网址(默认为最后一次访问)
        /// </summary>
        public void GoToUrl(string url = "")
        {
            Req.GoToUrl(url);
        }

        /// <summary>
        ///     刷新当前页
        /// </summary>
        public void Refresh()
        {
            GoToUrl("{0}?{1}", MvcReq.GetPageName(), MvcReq.GetParams());
        }
    }
}
