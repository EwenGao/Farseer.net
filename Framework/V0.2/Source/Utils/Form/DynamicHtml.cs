using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FS.Utils.Form
{
    /// <summary>
    /// 获取经过JS更改后的Html
    /// </summary>
    public partial class DynamicHtml : System.Windows.Forms.Form, IDisposable
    {
        bool IsCompleted = false;
        string Html;
        Encoding WebEncoding;

        public DynamicHtml(string url)
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = false;
            Navigate(url);
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
        }

        public DynamicHtml()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = false;
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
        }

        /// <summary>
        /// 导航到指定的Url
        /// </summary>
        /// <param name="url">地址</param>
        public void Navigate(string url)
        {
            IsCompleted = false;
            Html = "";
            webBrowser1.Navigate(url);
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.ReadyState != WebBrowserReadyState.Complete || (e.Url != webBrowser1.Url)) return;

            IsCompleted = true;

            WebEncoding = Encoding.GetEncoding(webBrowser1.Document.Encoding);
            Html = new StreamReader(webBrowser1.DocumentStream, WebEncoding).ReadToEnd();
        }

        /// <summary>
        /// 获取Js变量
        /// </summary>
        /// <param name="variableName">Js变量名称</param>
        public string GetJsVariable(string variableName)
        {
            Wait();
            var value = webBrowser1.Document.InvokeScript("eval", new string[] { variableName });
            if (value == null) { return string.Empty; }
            return value.ToString();
        }

        /// <summary>
        /// 获取源码
        /// </summary> 
        public string GetHtml()
        {
            Wait();
            return Html;
        }

        /// <summary>
        /// 获取动态源码
        /// </summary>
        public string GetDynamicHtml()
        {
            Wait();
            return webBrowser1.Document.Body.OuterHtml;
        }

        /// <summary>
        /// 获取页面编码
        /// </summary>
        public Encoding GetEncoding()
        {
            Wait();
            return WebEncoding;
        }

        void Wait()
        {
            var timeOut = 0;
            while (!IsCompleted)
            {
                Thread.Sleep(100);
                if (timeOut++ > 600) { IsCompleted = true; }
            }
        }

        public new void Dispose()
        {
            webBrowser1.Dispose();
            webBrowser1 = null;
            base.Dispose();
        }
    }
}