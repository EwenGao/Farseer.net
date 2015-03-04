using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
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