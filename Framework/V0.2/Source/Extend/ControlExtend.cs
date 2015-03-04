using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using FS.Core.Model;
using Control = System.Web.UI.Control;
using ListControl = System.Web.UI.WebControls.ListControl;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static class ControlExtend
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
        /// <summary>
        ///     获取父类RepeaterItem
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="Container">RepeaterItem</param>
        /// <returns></returns>
        public static T Parent<T>(this RepeaterItem Container)
        {
            return (T)((RepeaterItem)(Container.NamingContainer).NamingContainer).DataItem;
        }

        /// <summary>
        /// Container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Container"></param>
        /// <returns></returns>
        public static T Item<T>(this RepeaterItem Container)
        {
            return (T)(Container).DataItem;
        }

        /// <summary>
        ///  获取数据绑定上下文数据项。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="page">页面</param>
        public static T GetDataItem<T>(this Page page)
        {
            return (T)page.GetDataItem();
        }
        /// <summary>
        ///     绑定枚举
        /// </summary>
        /// <param name="rpt">Repeater</param>
        /// <param name="eumType">枚举</param>
        public static void Bind(this Repeater rpt, Type eumType)
        {
            rpt.DataSource = eumType.ToDictionary();
            rpt.DataBind();
        }

        /// <summary>
        ///     IEnumerable绑定到Repeater
        /// </summary>
        /// <param name="rpt">Repeater</param>
        /// <param name="lst">List列表</param>
        public static void Bind(this Repeater rpt, IEnumerable lst)
        {
            rpt.DataSource = lst;
            rpt.DataBind();
        }

        /// <summary>
        ///     IEnumerable绑定到Repeater
        /// </summary>
        /// <param name="rpt">QynRepeater</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="lst">IEnumerable</param>
        public static void Bind(this UI.Repeater rpt, IEnumerable lst, int recordCount = -1)
        {
            rpt.DataSource = lst;
            rpt.DataBind();

            if (recordCount > -1)
            {
                rpt.PageCount = recordCount;
            }
        }
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
            if (value == null) { return; }
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
        public static void Bind(this ListControl control, IEnumerable lst, object selectedValue = null, string dataTextField = "Caption", string dataValueField = "ID", string defShowText = null, object defShowValue = null)
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
        public static void Bind(this ListControl control, Type eumType, object selectedValue = null, string defShowText = null, object defShowValue = null)
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
                    foreach (ListItem lstItem in ((ListControl)item).Items)
                    {
                        lstItem.Enabled = true;
                        lstItem.Attributes.Add("onclick", "return false;");
                    }
                }
            }
        }
        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="lstInfo">要进行绑定的列表</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="RemoveID">不加载的节点（包括子节点）</param>
        public static void Bind(this DropDownList ddl, List<ModelCateInfo> lstInfo, object selectedValue = null, int RemoveID = -1)
        {
            ddl.Items.Clear();

            Bind(ddl, lstInfo, 0, 0, RemoveID);

            if (selectedValue != null)
            {
                ddl.SelectedItems(selectedValue);
            }
        }

        /// <summary>
        ///     递归绑定
        /// </summary>
        private static void Bind(DropDownList ddl, List<ModelCateInfo> lstInfo, int parentID = 0, int tagNum = 0, int RemoveID = 1)
        {
            if (lstInfo == null || lstInfo.Count == 0)
            {
                return;
            }

            var lstModelCateInfo = lstInfo.FindAll(o => o.ParentID == parentID);

            if (lstInfo == null || lstInfo.Count == 0)
            {
                return;
            }

            foreach (var info in lstModelCateInfo)
            {
                if (info.ID == RemoveID)
                {
                    continue;
                }
                ddl.Items.Add(new ListItem
                {
                    Value = info.ID.ToString(),
                    Text = new string('　', tagNum) + "├─" + info.Caption
                });
                Bind(ddl, lstInfo, info.ID.Value, tagNum + 1, RemoveID);
            }
        }

        /// <summary>
        ///     IEnumerable绑定到DataGridView
        /// </summary>
        /// <param name="dgv">DataGridView</param>
        /// <param name="lst">List列表</param>
        public static void Bind<T>(this DataGridView dgv, List<T> lst)
        {
            var bind = new BindingList<T>(lst);
            dgv.DataSource = bind;
        }

        /// <summary>
        ///     IEnumerable绑定到DataGridView
        /// </summary>
        /// <param name="dgv">DataGridView</param>
        /// <param name="lst">List列表</param>
        public static void Bind<T>(this DataGridView dgv, BindingList<T> lst, Action<object, ListChangedEventArgs> act = null)
        {
            if (act != null) { lst.ListChanged += (o, e) => { act(o, e); }; }
            dgv.DataSource = lst;
        }
    }
}