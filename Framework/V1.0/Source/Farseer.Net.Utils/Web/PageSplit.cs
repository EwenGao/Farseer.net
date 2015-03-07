using FS.Extend;
using FS.UI;
using FS.Utils.Common;

namespace FS.Utils.Web
{
    /// <summary>
    ///     分页工具
    /// </summary>
    public class PageSplit
    {
        /*
         {PageSize}         ：每页显示的数量
         {PageIndex}        ：当前索引
         {PageNextIndex}    ：下一页索引
         {PagePreIndex}     ：上一下索引
         {PageCount}        ：总共页数量
         {PageTotal}        ：总共数量
         {PageName}         ：当前路径页面名称
         {PageParam}        ：当前路径页面参数
         */

        /// <summary>
        /// 默认模板
        /// </summary>
        public string Template_Default = @"<a href=""{PageName}?PageIndex={PageIndex}&{PageParam}"">{PageIndex}</a>";
        /// <summary>
        /// 当前页
        /// </summary>
        public string Template_Active = @"<a href=""{PageName}?PageIndex={PageIndex}&{PageParam}"" class=""selected"">{PageIndex}</a>";
        /// <summary>
        /// 上一页
        /// </summary>
        public string Template_Pre = @"<a href=""{PageName}?PageIndex={PagePreIndex}&{PageParam}"" title=""上一页"">上一页</a>";
        /// <summary>
        /// 下一页
        /// </summary>
        public string Template_Next = @"<a href=""{PageName}?PageIndex={PageNextIndex}&{PageParam}"">下一页</a>";
        /// <summary>
        /// 首页
        /// </summary>
        public string Template_First = @"<a href=""{PageName}?PageIndex=1&{PageParam}"">首页</a>";
        /// <summary>
        /// 尾页
        /// </summary>
        public string Template_Last = @"<a href=""{PageName}?PageIndex={PageCount}&{PageParam}"">尾页</a>";
        /// <summary>
        /// 记录总数
        /// </summary>
        public string Template_Record = @"共<strong>{PageTotal}</strong>条记录&nbsp;&nbsp;<strong>{PageIndex}</strong>/<strong>{PageCount}</strong>页";

        /// <summary>
        /// 分布结果
        /// </summary>
        public struct SplitResult
        {
            public string Split { get; set; }
            public string Record { get; set; }
        }

        /// <summary>
        /// 模板替换
        /// </summary>
        /// <param name="template">模板</param>
        /// <param name="pageTotal">总共数量</param>
        /// <param name="pageCount">总共页数量</param>
        /// <param name="pageNextIndex">下一页索引</param>
        /// <param name="pagePreIndex">上一下索引</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="pageIndex">当前索引</param>
        /// <param name="pageName">当前路径页面名称</param>
        /// <param name="pageParam">当前路径页面参数</param>
        /// <returns></returns>
        private string Template(string template, int pageTotal, int pageCount, int pageNextIndex, int pagePreIndex, int pageSize, int pageIndex, string pageName = "", string pageParam = "")
        {
            template = template.Replace("{PageSize}", pageSize.ToString())
                .Replace("{PageIndex}", pageIndex.ToString())
                .Replace("{PageNextIndex}", pageNextIndex.ToString())
                .Replace("{PagePreIndex}", pagePreIndex.ToString())
                .Replace("{PageCount}", pageCount.ToString())
                .Replace("{PageTotal}", pageTotal.ToString())
                .Replace("{PageName}", pageName.ToString())
                .Replace("{PageParam}", pageParam);
            return template;
        }

        /// <summary>
        /// 分页函数（模板）
        /// </summary>
        /// <param name="pageTotal">总记录数</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageName">页面名称</param>
        public SplitResult Split(int pageTotal, int pageSize, int pageIndex, string pageName, string pageParam = "")
        {
            //总页数
            var pageCount = 0;
            //下一页
            var nextIndex = 0;
            //上一页
            var preIndex = 0;
            //开始页码
            var startCount = 0;
            //结束页码
            var endCount = 0;
            // 将返回的结果
            var sr = new SplitResult();
            var sp = new StrPlus();

            if (pageIndex < 1) { pageIndex = 1; }

            //计算总页数
            if (pageSize != 0)
            {
                pageCount = (pageTotal / pageSize);
                pageCount = ((pageTotal % pageSize) != 0 ? pageCount + 1 : pageCount);
                pageCount = (pageCount == 0 ? 1 : pageCount);
            }
            nextIndex = pageIndex + 1;
            preIndex = pageIndex - 1;

            if (preIndex < 1) { preIndex = 1; }
            if (nextIndex > pageCount) { nextIndex = pageCount; }

            //中间页起始序号
            startCount = (pageIndex + 5) > pageCount ? pageCount - 9 : pageIndex - 4;

            //中间页终止序号
            endCount = pageIndex < 5 ? 10 : pageIndex + 5;

            //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
            if (startCount < 1) { startCount = 1; }

            //页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
            if (pageCount < endCount) { endCount = pageCount; }

            // 首页
            sp += Template(Template_First, pageTotal, pageCount, nextIndex, preIndex, pageSize, pageIndex, pageName, pageParam);
            // 上一页
            sp += Template(Template_Pre, pageTotal, pageCount, nextIndex, preIndex, pageSize, pageIndex, pageName, pageParam);
            // 页码列表
            for (var i = startCount; i <= endCount; i++)
            {
                if (i != pageIndex || Template_Active.IsNullOrEmpty()) { sp += Template(Template_Default, pageTotal, pageCount, nextIndex, preIndex, pageSize, i, pageName, pageParam); }
                else { sp += Template(Template_Active, pageTotal, pageCount, nextIndex, preIndex, pageSize, i, pageName, pageParam); }
            }
            // 下一页 
            sp += Template(Template_Next, pageTotal, pageCount, nextIndex, pageIndex, pageSize, pageIndex, pageName, pageParam);
            // 尾页
            sp += Template(Template_Last, pageTotal, pageCount, nextIndex, pageIndex, pageSize, pageIndex, pageName, pageParam);

            sr.Split = sp;
            sr.Record = Template(Template_Record, pageTotal, pageCount, nextIndex, preIndex, pageSize, pageIndex);

            return sr;
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

            if (pageIndex < 1) { pageIndex = 1; }

            pageName += pageName.IndexOf('?') != -1 ? "&" : "?";

            //计算总页数
            if (pageSize != 0)
            {
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
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
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
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
                allCurrentPage = (recordCount / pageSize);
                allCurrentPage = ((recordCount % pageSize) != 0 ? allCurrentPage + 1 : allCurrentPage);
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