using System;
using System.Collections;
using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
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
    }
}