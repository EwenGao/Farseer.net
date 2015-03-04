using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
        /// <summary>
        ///     获取ListBox的值
        /// </summary>
        /// <param name="control">ListControl控件</param>
        public static List<T> GetValue<T>(this ListControl control)
        {
            var lst = new List<T>();
            if (control is CheckBoxList)
            {
                foreach (ListItem item in control.Items)
                {
                    if (item.Selected)
                    {
                        lst.Add(item.Value.ConvertType<T>());
                    }
                }
            }
            else
            {
                foreach (ListItem item in control.Items)
                {
                    lst.Add(item.Value.ConvertType<T>());
                }
            }
            return lst;
        }

        /// <summary>
        ///     设置ListBox的值
        /// </summary>
        /// <param name="control">ListControl控件</param>
        /// <param name="lst">要设置的值</param>
        public static void SetValue<T>(this ListControl control, List<T> lst)
        {
            if (control is CheckBoxList)
            {
                foreach (ListItem item in control.Items)
                {
                    if (!item.Value.IsType<T>())
                    {
                        continue;
                    }
                    item.Selected = lst.Exists(o => o.ToString() == item.Value);
                }
            }
            else
            {
                foreach (var item in lst)
                {
                    control.Items.Add(item.ToString());
                }
            }
        }

        /// <summary>
        ///     插入项
        /// </summary>
        /// <param name="control">control控件</param>
        /// <param name="text">显示的名称</param>
        /// <param name="value">保存的值</param>
        /// <param name="index">插入的项索引</param>
        public static void InsertItem(this ListControl control, object value, string text = "请选择", int index = 0)
        {
            if (value == null)
            {
                return;
            }
            control.Items.Insert(index, new ListItem { Text = text, Value = value.ToString() });
        }

        /// <summary>
        ///     选择项
        /// </summary>
        /// <param name="control">ListControl控件</param>
        /// <param name="selectedValue">选择项</param>
        public static void SelectedItems(this ListControl control, object selectedValue)
        {
            var selValue = string.Empty;

            if (selectedValue is Enum)
            {
                selValue = Convert.ToString((int)selectedValue);
            }
            else if (selectedValue is bool)
            {
                selValue = selectedValue.ToString().ToLower();
            }
            else
            {
                selValue = selectedValue == null ? string.Empty : selectedValue.ToString();
            }

            // 清除当前选择的项
            control.ClearSelection();

            if (control.Items.FindByValue(selValue) != null)
            {
                control.Items.FindByValue(selValue).Selected = true;
            }
            else if (control.Items.FindByText(selValue) != null)
            {
                control.Items.FindByText(selValue).Selected = true;
            }
        }

        /// <summary>
        ///     string[]绑定到WebControl
        /// </summary>
        /// <param name="control">要绑定的ddl</param>
        /// <param name="strs">源数据</param>
        /// <param name="selectedValue">默认选择值</param>
        public static void Bind<T>(this ListControl control, T[] strs, object selectedValue = null) where T : struct
        {
            if (strs == null)
            {
                return;
            }
            foreach (var str in strs)
            {
                control.Items.Add(str.ToString());
            }

            if (selectedValue != null)
            {
                control.SelectedItems(selectedValue);
            }
        }

        /// <summary>
        ///     IEnumerable绑定到WebControl
        /// </summary>
        /// <param name="control">要绑定的ddl</param>
        /// <param name="lst">源数据</param>
        /// <param name="dataTextField">绑定的文本字段</param>
        /// <param name="dataValueField">绑定的值字段</param>
        /// <param name="defShowText">第一行显示的文字</param>
        /// <param name="defShowValue">第一行显示的值</param>
        /// <param name="selectedValue">默认选择值</param>
        public static void Bind(this ListControl control, IEnumerable lst, object selectedValue = null,
                                string dataTextField = "Caption", string dataValueField = "ID",
                                string defShowText = null, object defShowValue = null)
        {
            control.DataSource = lst;
            control.DataTextField = dataTextField;
            control.DataValueField = dataValueField;
            control.DataBind();

            if (control is DropDownList && control.Items.Count > 0)
            {
                control.SelectedIndex = 0;
            }

            control.InsertItem(defShowValue, defShowText);
            if (selectedValue != null)
            {
                control.SelectedItems(selectedValue);
            }
        }

        /// <summary>
        ///     Eume绑定到WebControl
        /// </summary>
        /// <param name="control">要绑定的ddl</param>
        /// <param name="eumType">枚举的Type</param>
        /// <param name="defShowText">第一行要显示的文字</param>
        /// <param name="defShowValue">第一行要显示的值</param>
        /// <param name="selectedValue">默认选择值</param>
        public static void Bind(this ListControl control, Type eumType, object selectedValue = null,
                                string defShowText = null, object defShowValue = null)
        {
            control.DataSource = eumType.ToListItem();
            control.DataValueField = "Value";
            control.DataTextField = "Text";
            control.DataBind();
            if (control.Items.Count > 0)
            {
                control.SelectedIndex = 0;
            }

            control.InsertItem(defShowValue, defShowText);
            if (selectedValue != null)
            {
                control.SelectedItems(selectedValue);
            }
        }

        /// <summary>
        ///     WinForm绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="lst">List列表</param>
        /// <param name="dataTextField">显示名称</param>
        /// <param name="dataValueField">值</param>
        public static void Bind(this System.Windows.Forms.ListControl control, IEnumerable lst, string dataTextField = "Caption", string dataValueField = "ID")
        {
            control.DisplayMember = dataTextField;
            control.ValueMember = dataValueField;

            control.DataSource = lst;
        }

        /// <summary>
        ///     WinForm绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="eumType">枚举类型</param>
        public static void Bind(this System.Windows.Forms.ListControl control, Type eumType)
        {
            var lst = new List<string>();
            foreach (var item in eumType.ToDictionary()) { lst.Add(item.Value); }

            control.DataSource = lst;
        }

        /// <summary>
        ///     string[]绑定到WebControl
        /// </summary>
        /// <param name="control">要绑定的ddl</param>
        /// <param name="trueCaption">值为是的提示</param>
        /// <param name="falseCaption">值为不是的提示</param>
        /// <param name="NoSelectCaption">未选择的提示</param>
        public static void Bind(this ListControl control, string trueCaption = "是", string falseCaption = "否", string NoSelectCaption = "")
        {
            if (!NoSelectCaption.IsNullOrEmpty()) { control.Items.Add(new ListItem { Value = "", Text = NoSelectCaption }); }
            control.Items.Add(new ListItem { Value = "false", Text = falseCaption });
            control.Items.Add(new ListItem { Value = "true", Text = trueCaption });

            // 清除当前选择的项
            control.ClearSelection();
            control.SelectedIndex = 0;
        }
    }
}