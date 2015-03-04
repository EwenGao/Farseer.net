using System.Collections.Generic;
using System.Web.Routing;
using FS.Mvc;

namespace System.Web.Mvc.Html
{
    /// <summary>
    ///     Mvc 链接扩展
    /// </summary>
    public static class LinkExtensions
    {
        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, new RouteValueDictionary(),
                                         new RouteValueDictionary(), requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        object routeValues, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, new RouteValueDictionary(routeValues),
                                         new RouteValueDictionary(), requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        string controllerName, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(),
                                         new RouteValueDictionary(), requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        RouteValueDictionary routeValues, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, routeValues, new RouteValueDictionary(),
                                         requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        object routeValues, object htmlAttributes, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, new RouteValueDictionary(routeValues),
                                         new RouteValueDictionary(htmlAttributes), requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes,
                                        bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, routeValues, htmlAttributes, requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        string controllerName, object routeValues, object htmlAttributes,
                                        bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues),
                                         new RouteValueDictionary(htmlAttributes), requireAbsoluteUrl);
        }

        /// <summary>
        ///     链接地址
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="requireAbsoluteUrl"></param>
        /// <returns></returns>
        public static string ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                        string controllerName, RouteValueDictionary routeValues,
                                        IDictionary<string, object> htmlAttributes, bool requireAbsoluteUrl)
        {
            if (requireAbsoluteUrl)
            {
                HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
                var routeData = RouteTable.Routes.GetRouteData(currentContext);

                routeData.Values["controller"] = controllerName;
                routeData.Values["action"] = actionName;

                var domainRoute = routeData.Route as DomainRoute;
                if (domainRoute != null)
                {
                    var domainData = domainRoute.GetDomainData(new RequestContext(currentContext, routeData),
                                                                      routeData.Values);
                    return
                        htmlHelper.ActionLink(linkText, actionName, controllerName, domainData.Protocol,
                                              domainData.HostName, domainData.Fragment, routeData.Values, null)
                                  .ToHtmlString();
                }
            }
            return
                htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
        }
    }
}