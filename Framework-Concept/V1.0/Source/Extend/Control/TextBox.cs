using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
        /// <summary>
        ///     清除空格
        /// </summary>
        /// <param name="control">TextBox控件</param>
        public static string Trim(this System.Windows.Forms.TextBox control)
        {
            control.Text = control.Text.Trim();
            return control.Text;
        }

        /// <summary>
        ///     清除空格
        /// </summary>
        /// <param name="control">TextBox控件</param>
        public static string Trim(this System.Web.UI.WebControls.TextBox control)
        {
            control.Text = control.Text.Trim();
            return control.Text;
        }
    }
}