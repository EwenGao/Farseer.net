using FS.Extend;
using FS.UI;

namespace FS.Utils.Web
{
    /// <summary>
    ///     分页工具
    /// </summary>
    public abstract class PageSplit
    {
        /// <summary>
        ///     分页函数
        /// </summary>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageName">页面名称</param>
        /// <param name="cssSelected">当前页面的CSS</param>
        /// <param name="cssNoSelected">非当前页面的CSS</param>
        /// <param name="isShowJump">是否允许跳转</param>
        /// <param name="isShowRecordCount">是否显示总记录数</param>
        public static string AspxSplit(int recordCount, int pageSize, int pageIndex, bool isShowRecordCount = true,
                                       bool isShowJump = false, string pageName = "", string cssSelected = "",
                                       string cssNoSelected = "")
        {
            //总页数
            var allCurrentPage = 0;
            //下一页
            var next = 0;
            //上一页
            var pre = 0;
            //开始页码
            var startCount = 0;
            //结束页码
            var endCount = 0;
            var currentPageStr = "";

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            pageName += pageName.IndexOf('?') != -1 ? "&" : "?";

            //计算总页数
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount/pageSize);
                allCurrentPage = ((recordCount%pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            next = pageIndex + 1;
            pre = pageIndex - 1;

            //中间页起始序号
            startCount = (pageIndex + 5) > allCurrentPage ? allCurrentPage - 9 : pageIndex - 4;

            //中间页终止序号
            endCount = pageIndex < 5 ? 10 : pageIndex + 5;

            //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
            if (startCount < 1)
            {
                startCount = 1;
            }

            //页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
            if (allCurrentPage < endCount)
            {
                endCount = allCurrentPage;
            }

            #region 记录

            if (isShowRecordCount)
            {
                currentPageStr +=
                    string.Format("<strong>{0}</strong>条记录&nbsp;&nbsp;<strong>{1}</strong>/<strong>{2}</strong>页",
                                  recordCount, pageIndex, allCurrentPage);
                currentPageStr += "&nbsp;&nbsp;";
            }

            #endregion

            #region 首页

            if (pageIndex != 1)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"{0}pageSize={1}&pageIndex=1 \" title=\"首页\"><span class=\"{2}\">首页</span></a>",
                        pageName, pageSize, cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"{0}\">首页</span></a>", cssNoSelected);
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 上一页

            if (pageIndex > 1)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"{0}pageSize={1}&pageIndex={2} \" title=\"上一页\" ><span class=\"{3}\">上一页</span></a>",
                        pageName, pageSize, pre, cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"Pagination\">上一页</span></a>");
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 页码列表

            //当页码数大于0时, 则显示页码
            if (endCount > 0)
            {
                //中间页处理, 这个增加时间复杂度，减小空间复杂度
                for (var i = startCount; i <= endCount; i++)
                {
                    currentPageStr +=
                        string.Format(
                            "<a href=\"{0}pageSize={1}&pageIndex={2} \" title=\"第{2}页\"><span class=\"{3}\">{2}</span></a> ",
                            pageName, pageSize, i, i == pageIndex ? cssSelected : cssNoSelected);
                }
                currentPageStr += "&nbsp;";
            }

            #endregion

            #region 下一页

            if (pageIndex != allCurrentPage)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"{0}pageSize={1}&pageIndex={2} \" title=\"下一页\" ><span class=\"{3}\">下一页</span></a>",
                        pageName, pageSize, next, cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"{0}\">下一页</span></a>", cssNoSelected);
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 尾页

            if (pageIndex < allCurrentPage)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"{0}pageSize={1}&pageIndex={2} \" title=\"尾页\" ><span class=\"{3}\">尾页</span></a>",
                        pageName, pageSize, allCurrentPage, cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"{0}\">尾页</span></a>", cssNoSelected);
            }
            //currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 页码列表Select

            //当页码数大于1时, 则显示页码
            //if (endCount > 1)
            //{
            //    currentPageStr += "&nbsp; &nbsp;<select id='selPagination' name='selPagination' onchange=\"javascript:qyn_jumpMenuPagination(this);\">";
            //    string selected = "";

            //    //中间页处理, 这个增加时间复杂度，减小空间复杂度
            //    for (int i = startCount; i <= endCount; i++)
            //    {
            //        selected = i == pageIndex ? "selected=\"selected\"" : "";
            //        currentPageStr += string.Format("<option value=\"{0}|{1}\" {2}>第{1}页</option>", pageSize,i,selected);
            //    }
            //    currentPageStr += "</select>";
            //}

            #endregion

            #region 页面跳转

            if (isShowJump)
            {
                currentPageStr +=
                    string.Format(
                        "&nbsp;&nbsp;跳转&nbsp;&nbsp;<input id=\"txtPageIndex\" name=\"txtPageIndex\" type=\"text\" value=\"{0}\" style=\"width:35px;text-align:center;\" onblur=\"location.href='{1}pageIndex=' + this.value + '&pageSize={2}'\" onclick=\"this.select();\" style=\"text-align:center\">&nbsp;&nbsp;页",
                        pageIndex >= allCurrentPage ? pageIndex : pageIndex + 1, pageName, pageSize);
            }

            #endregion

            return currentPageStr;
        }


        /// <summary>
        ///     分页函数
        /// </summary>
        /// <param name="rptList">分页控件</param>
        public static string AspxSplit(Repeater rptList)
        {
            return AspxSplit(rptList.PageCount, rptList.PageSize, rptList.PageIndex, rptList.IsShowRecordCount, rptList.IsShowJump, rptList.PageUrl, rptList.Selected, rptList.NoSelected);
        }

        /// <summary>
        ///     分页函数
        /// </summary>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageName">页面名称{$=PageIndex#}</param>
        /// <param name="cssSelected">当前页面的CSS</param>
        /// <param name="cssNoSelected">非当前页面的CSS</param>
        /// <param name="isShowJump">是否允许跳转</param>
        /// <param name="isShowRecordCount">是否显示总记录数</param>
        public static string HtmlSplit(int recordCount, int pageSize, int pageIndex, bool isShowRecordCount = true,
                                       bool isShowJump = false, string pageName = "", string cssSelected = "",
                                       string cssNoSelected = "")
        {
            //总页数
            var allCurrentPage = 0;
            //下一页
            var next = 0;
            //上一页
            var pre = 0;
            //开始页码
            var startCount = 0;
            //结束页码
            var endCount = 0;
            var currentPageStr = "";

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }


            if (pageName.IndexOf("{$=PageIndex#}") == -1)
            {
                var index = pageName.IndexOf(".");
                if (index > -1)
                {
                    pageName = string.Format("{0}.{1}",
                                             pageName.SubString(0, index) + "_{$=PageIndex#}",
                                             pageName.SubString(index + 1, -1));
                }
                else
                {
                    pageName = "index_{$=PageIndex#}.html";
                }
                //else
                //{
                //    if (!pageName.EndsWith("&") && !pageName.EndsWith("?"))
                //    {
                //        pageName += pageName.IndexOf('?') != -1 ? "&" : "?";
                //    }
                //    pageName += "PageIndex={$=PageIndex#}";
                //}
            }
            // pageName = Url.ParmsEncode(pageName);

            //计算总页数
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount/pageSize);
                allCurrentPage = ((recordCount%pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            next = pageIndex + 1;
            pre = pageIndex - 1;

            //中间页起始序号
            startCount = (pageIndex + 5) > allCurrentPage ? allCurrentPage - 9 : pageIndex - 4;

            //中间页终止序号
            endCount = pageIndex < 5 ? 10 : pageIndex + 5;

            //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
            if (startCount < 1)
            {
                startCount = 1;
            }

            //页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
            if (allCurrentPage < endCount)
            {
                endCount = allCurrentPage;
            }

            #region 记录

            if (isShowRecordCount)
            {
                currentPageStr +=
                    string.Format("<strong>{0}</strong>条记录&nbsp;&nbsp;<strong>{1}</strong>/<strong>{2}</strong>页",
                                  recordCount, pageIndex, allCurrentPage);
                currentPageStr += "&nbsp;&nbsp;";
            }

            #endregion

            #region 首页

            if (pageIndex != 1)
            {
                currentPageStr += string.Format("<a href=\"{0}\" title=\"首页\"><span class=\"{1}\">首页</span></a>",
                                                ReplaceParms(pageName, pageSize, 1), cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"{0}\">首页</span></a>", cssNoSelected);
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 上一页

            if (pageIndex > 1)
            {
                currentPageStr += string.Format("<a href=\"{0}\" title=\"上一页\" ><span class=\"{1}\">上一页</span></a>",
                                                ReplaceParms(pageName, pageSize, pre), cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"Pagination\">上一页</span></a>");
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 页码列表

            //当页码数大于0时, 则显示页码
            if (endCount > 0)
            {
                //中间页处理, 这个增加时间复杂度，减小空间复杂度
                for (var i = startCount; i <= endCount; i++)
                {
                    currentPageStr +=
                        string.Format("<a href=\"{0} \" title=\"第{1}页\"><span class=\"{2}\">{1}</span></a> ",
                                      ReplaceParms(pageName, pageSize, i), i,
                                      i == pageIndex ? cssSelected : cssNoSelected);
                }
                currentPageStr += "&nbsp;";
            }

            #endregion

            #region 下一页

            if (pageIndex != allCurrentPage)
            {
                currentPageStr += string.Format("<a href=\"{0}\" title=\"下一页\" ><span class=\"{1}\">下一页</span></a>",
                                                ReplaceParms(pageName, pageSize, next), cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"{0}\">下一页</span></a>", cssNoSelected);
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 尾页

            if (pageIndex < allCurrentPage)
            {
                currentPageStr += string.Format("<a href=\"{0}\" title=\"尾页\" ><span class=\"{1}\">尾页</span></a>",
                                                ReplaceParms(pageName, pageSize, allCurrentPage), cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<a><span class=\"{0}\">尾页</span></a>", cssNoSelected);
            }
            //currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 页面跳转

            if (isShowJump)
            {
                currentPageStr +=
                    string.Format(
                        "&nbsp;&nbsp;跳转&nbsp;&nbsp;<input id=\"txtPageIndex\" name=\"txtPageIndex\" type=\"text\" value=\"{0}\" style=\"width:35px;text-align:center;\" onblur=\"location.href='{1}'\" onclick=\"this.select();\" style=\"text-align:center\">&nbsp;&nbsp;页",
                        pageIndex >= allCurrentPage ? pageIndex : pageIndex + 1,
                        ReplaceParms(pageName, pageSize.ToString(), "' + this.value + '"), pageSize);
            }

            #endregion

            return currentPageStr;
        }

        /// <summary>
        ///     分页函数
        /// </summary>
        /// <param name="rptList">分页控件</param>
        public static string HtmlSplit(Repeater rptList)
        {
            return HtmlSplit(rptList.PageCount, rptList.PageSize, rptList.PageIndex, rptList.IsShowRecordCount, rptList.IsShowJump, rptList.PageUrl, rptList.Selected, rptList.NoSelected);
        }

        /// <summary>
        ///     分页函数
        /// </summary>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageName">页面名称</param>
        /// <param name="cssSelected">当前页面的CSS</param>
        /// <param name="cssNoSelected">非当前页面的CSS</param>
        /// <param name="isShowJump">是否允许跳转</param>
        /// <param name="isShowRecordCount">是否显示总记录数</param>
        /// <returns></returns>
        public static string AjaxSplit(int recordCount, int pageSize, int pageIndex, bool isShowRecordCount = true,
                                       bool isShowJump = false, string pageName = "", string cssSelected = "",
                                       string cssNoSelected = "")
        {
            //总页数
            var allCurrentPage = 0;
            //下一页
            var next = 0;
            //上一页
            var pre = 0;
            //开始页码
            var startCount = 0;
            //结束页码
            var endCount = 0;
            var currentPageStr = "";

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            pageName += pageName.IndexOf('?') != -1 ? "&" : "?";

            //计算总页数
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount/pageSize);
                allCurrentPage = ((recordCount%pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
                allCurrentPage = (allCurrentPage == 0 ? 1 : allCurrentPage);
            }
            next = pageIndex + 1;
            pre = pageIndex - 1;

            //中间页起始序号
            startCount = (pageIndex + 5) > allCurrentPage ? allCurrentPage - 9 : pageIndex - 4;

            //中间页终止序号
            endCount = pageIndex < 5 ? 10 : pageIndex + 5;

            //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
            if (startCount < 1)
            {
                startCount = 1;
            }

            //页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
            if (allCurrentPage < endCount)
            {
                endCount = allCurrentPage;
            }

            #region 记录

            if (isShowRecordCount)
            {
                currentPageStr +=
                    string.Format("<strong>{0}</strong>条记录&nbsp;&nbsp;<strong>{1}</strong>/<strong>{2}</strong>页",
                                  recordCount, pageIndex, allCurrentPage);
                currentPageStr += "&nbsp;&nbsp;";
            }

            #endregion

            #region 首页

            if (pageIndex != 1)
            {
                currentPageStr +=
                    string.Format("<a href=\"javascript:GetList(1);\" title=\"首页\"><span class=\"{0}\">首页</span></a>",
                                  cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<span class=\"{0}\">首页</span>", cssNoSelected);
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 上一页

            if (pageIndex > 1)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"javascript:GetList({0});\" title=\"上一页\" ><span class=\"{1}\">上一页</span></a>", pre,
                        cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<span class=\"Pagination\">上一页</span>");
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 页码列表

            //当页码数大于0时, 则显示页码
            if (endCount > 0)
            {
                //中间页处理, 这个增加时间复杂度，减小空间复杂度
                for (var i = startCount; i <= endCount; i++)
                {
                    currentPageStr +=
                        string.Format(
                            "<a href=\"javascript:GetList({0});\" title=\"第{0}页\"><span class=\"{1}\">{0}</span></a> ",
                            i, i == pageIndex ? cssSelected : cssNoSelected);
                }
                currentPageStr += "&nbsp;";
            }

            #endregion

            #region 下一页

            if (pageIndex != allCurrentPage)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"javascript:GetList({0});\" title=\"下一页\" ><span class=\"{1}\">下一页</span></a>", next,
                        cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<span class=\"{0}\">下一页</span>", cssNoSelected);
            }
            currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 尾页

            if (pageIndex < allCurrentPage)
            {
                currentPageStr +=
                    string.Format(
                        "<a href=\"javascript:GetList({0});\" title=\"尾页\" ><span class=\"{1}\">尾页</span></a>",
                        allCurrentPage, cssNoSelected);
            }
            else
            {
                currentPageStr += string.Format("<span class=\"{0}\">尾页</span>", cssNoSelected);
            }
            //currentPageStr += "&nbsp;&nbsp;";

            #endregion

            #region 页面跳转

            if (isShowJump)
            {
                currentPageStr +=
                    string.Format(
                        "&nbsp;&nbsp;跳转&nbsp;&nbsp;<input id=\"txtPageIndex\" name=\"txtPageIndex\" type=\"text\" value=\"{0}\" style=\"width:35px;text-align:center;\" onblur=\"GetList(this.value);\" onclick=\"this.select();\" style=\"text-align:center\">&nbsp;&nbsp;页",
                        pageIndex >= allCurrentPage ? pageIndex : pageIndex + 1);
            }

            #endregion

            return currentPageStr;
        }

        /// <summary>
        ///     分页函数
        /// </summary>
        /// <param name="rptList">分页控件</param>
        public static string AjaxSplit(Repeater rptList)
        {
            return AjaxSplit(rptList.PageCount, rptList.PageSize, rptList.PageIndex, rptList.IsShowRecordCount, rptList.IsShowJump, rptList.PageUrl, rptList.Selected, rptList.NoSelected);
        }

        private static string ReplaceParms(string pageName, int pageSize, int pageIndex)
        {
            return ReplaceParms(pageName, pageSize.ToString(), pageIndex.ToString());
        }

        private static string ReplaceParms(string pageName, string pageSize, string pageIndex)
        {
            return pageName.Replace("{$=PageSize#}", pageSize).Replace("{$=PageIndex#}", pageIndex);
        }
    }
}