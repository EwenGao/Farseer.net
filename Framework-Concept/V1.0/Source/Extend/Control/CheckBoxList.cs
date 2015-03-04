using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
        /// <summary>
        ///     获取CheckBoxList中，选中后的枚举值
        /// </summary>
        /// <param name="control">CheckBoxList控件</param>
        public static int GetValue(this CheckBoxList control)
        {
            var enumValue = 0;

            foreach (ListItem item in control.Items)
            {
                if (item.Selected)
                {
                    enumValue |= item.Value.ConvertType<int>();
                }
            }
            return enumValue;
        }

        /// <summary>
        ///     获取CheckBoxList中，选中后的枚举值
        /// </summary>
        /// <param name="control">CheckBoxList控件</param>
        /// <param name="value">设置的值</param>
        public static void SetValue(this CheckBoxList control, int value)
        {
            foreach (ListItem item in control.Items)
            {
                var val = item.Value.ConvertType(0);
                item.Selected = (val & value) == val;
            }
        }
    }
}