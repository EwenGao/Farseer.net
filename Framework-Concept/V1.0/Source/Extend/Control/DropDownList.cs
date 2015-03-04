using System.Collections.Generic;
using System.Web.UI.WebControls;
using FS.Core.Model;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="lstInfo">要进行绑定的列表</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="RemoveID">不加载的节点（包括子节点）</param>
        public static void Bind(this DropDownList ddl, List<BaseCateInfo> lstInfo, object selectedValue = null,
                                int RemoveID = -1)
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
        private static void Bind(DropDownList ddl, List<BaseCateInfo> lstInfo, int parentID = 0, int tagNum = 0,
                                 int RemoveID = 1)
        {
            if (lstInfo == null || lstInfo.Count == 0)
            {
                return;
            }

            var lstBaseCateInfo = lstInfo.FindAll(o => o.ParentID == parentID);

            if (lstInfo == null || lstInfo.Count == 0)
            {
                return;
            }

            foreach (var info in lstBaseCateInfo)
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
                Bind(ddl, lstInfo, info.ID, tagNum + 1, RemoveID);
            }
        }
    }
}