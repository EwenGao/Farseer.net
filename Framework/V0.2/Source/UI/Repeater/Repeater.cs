using System.Web.UI;
using FS.Extend;
using FS.Utils.Web;

[assembly: TagPrefix("FS.UI", "FS")]

namespace FS.UI
{
    /// <summary>
    ///     带分页
    /// </summary>
    [ToolboxData(
        "<{0}:Repeater ID=\"rptList\" runat=server><ItemTemplate><tr><td><%# ((string)Container.DataItem)%></td></tr></ItemTemplate><PaginationHtml><tr class=\"tdbg\" align=\"center\" style=\"height: 28px;\"><td colspan=\"12\"><Pagination /></td></tr></PaginationHtml></{0}:Repeater>"
        )]
    public partial class Repeater : System.Web.UI.WebControls.Repeater
    {
        /// <summary>
        ///     输出Html
        /// </summary>
        /// <param name="writer">HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (!IsShowNotEnough && m_PageCount < 2)
            {
                return;
            }

            var HtmlSplit = string.Empty;
            switch (PageType)
            {
                case eumPageType.Html:
                    HtmlSplit = PageSplit.HtmlSplit(PageCount, PageSize, PageIndex, IsShowRecordCount, IsShowJump,
                                                    PageUrl + UrlParms, Selected, NoSelected);
                    break;
                case eumPageType.Ajax:
                    HtmlSplit = PageSplit.AjaxSplit(PageCount, PageSize, PageIndex, IsShowRecordCount, IsShowJump,
                                                    PageUrl + UrlParms, Selected, NoSelected);
                    break;
                default:
                    HtmlSplit = PageSplit.AspxSplit(PageCount, PageSize, PageIndex, IsShowRecordCount, IsShowJump,
                                                    PageUrl + UrlParms, Selected, NoSelected);
                    break;
            }
            if (!ChangePageSize)
            {
                HtmlSplit = HtmlSplit.ClearString(string.Format("pageSize={0}&", PageSize));
                HtmlSplit = HtmlSplit.ClearString(string.Format("/?pageSize={0}", PageSize));
                HtmlSplit = HtmlSplit.ClearString(string.Format("pageSize={0}", PageSize));
            }

            if (Languange == LanguageType.English)
            {
                HtmlSplit = HtmlSplit.Replace("条记录", "RecordCount")
                                     .Replace("上一页", "Previous")
                                     .Replace("下一页", "Next")
                                     .Replace("首页", "First")
                                     .Replace("尾页", "End")
                                     .Replace("跳转", "Jump")
                                     .Replace("页", "Page");
            }
            writer.WriteLine(PaginationHtml.Replace("<Pagination />", HtmlSplit).Replace("<pagination />", HtmlSplit));
        }
    }
}