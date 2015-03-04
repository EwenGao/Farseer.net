using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Control = System.Web.UI.Control;
using ListControl = System.Web.UI.WebControls.ListControl;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
        /// <summary>
        ///     将ListItem 转换为List
        /// </summary>
        /// <typeparam name="T">指定要转换的基本类型</typeparam>
        /// <param name="lstItem">ListItemCollection</param>
        public static List<T> ToList<T>(this ListItemCollection lstItem) where T : struct
        {
            var lst = new List<T>();
            foreach (ListItem item in lstItem)
            {
                lst.Add(item.Value.ConvertType<T>());
            }
            return lst;
        }

        /// <summary>
        ///     WinForm绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="eumType">枚举类型</param>
        public static void Bind(this DataGridViewComboBoxColumn control, Type eumType)
        {
            control.DataSource = eumType.ToDictionary();
            control.ValueMember = "Value";
            control.DisplayMember = "Text";
        }

        /// <summary>
        ///     设置冻结的ListControl的颜色不为灰
        /// </summary>
        /// <param name="controls"></param>
        public static void SetDisEnabledColor(this ControlCollection controls)
        {
            if (controls.Count == 0)
            {
                return;
            }
            foreach (Control item in controls)
            {
                SetDisEnabledColor(item.Controls);
                if (item is ListControl)
                {
                    foreach (ListItem lstItem in ((ListControl) item).Items)
                    {
                        lstItem.Enabled = true;
                        lstItem.Attributes.Add("onclick", "return false;");
                    }
                }
            }
        }
    }
}