using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FS.Mvc
{
    /// <summary>
    ///     路由数据
    /// </summary>
    public class DomainRoute : Route
    {
        private Regex domainRegex;
        private Regex pathRegex;

        /// <summary>
        ///     路由设置
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="url">请求的地址</param>
        public DomainRoute(string domain, string url)
            : base(url, new MvcRouteHandler())
        {
            Domain = domain;
        }

        /// <summary>
        ///     路由设置
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="url">请求的地址</param>
        /// <param name="defaults">设置默认值</param>
        public DomainRoute(string domain, string url, object defaults)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            Domain = domain;
        }

        /// <summary>
        ///     路由设置
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="url">请求的地址</param>
        /// <param name="defaults">设置默认值</param>
        /// <param name="constraints">正则</param>
        public DomainRoute(string domain, string url, object defaults, object constraints)
            : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler())
        {
            Domain = domain;
        }

        /// <summary>
        ///     路由设置
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="url">请求的地址</param>
        /// <param name="defaults">设置默认值</param>
        /// <param name="constraints">正则</param>
        /// <param name="dataTokens"></param>
        public DomainRoute(string domain, string url, object defaults, object constraints, object dataTokens)
            : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new RouteValueDictionary(dataTokens), new MvcRouteHandler())
        {
            Domain = domain;
        }

        /// <summary>
        ///     域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        ///     获取路由数据
        /// </summary>
        /// <param name="httpContext">请求的上下文</param>
        /// <returns></returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // 获取请求的域名端口
            var requestDomain = httpContext.Request.Headers["host"];
            // 获取请求的路径
            var requestPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;

            // 构造 regex
            domainRegex = CreateRegex(Domain);
            pathRegex = CreateRegex(Url);

            // 域名无法获取时，再次获取
            if (string.IsNullOrEmpty(requestDomain))
            {
                requestDomain = httpContext.Request.Url.Host;
            }
            // 移除端口
            if (requestDomain.IndexOf(":") > 0) { requestDomain = requestDomain.Substring(0, requestDomain.IndexOf(":")); }


            // 匹配域名
            var domainMatch = domainRegex.Match(requestDomain);
            // 匹配路径
            var pathMatch = pathRegex.Match(requestPath);

            // 如果域名，路径不匹配，直接返回
            if (!domainMatch.Success || !pathMatch.Success) { return null; }

            var data = new RouteData(this, RouteHandler);

            // 添加默认选项
            if (Defaults != null)
            {
                foreach (var item in Defaults) { data.Values[item.Key] = item.Value; }
            }

            // 匹配域名
            GetRouteData(data, domainMatch, domainRegex);
            // 匹配路径
            GetRouteData(data, pathMatch, pathRegex);

            return data;
        }

        /// <summary>
        ///     获取虚拟路径
        /// </summary>
        /// <param name="requestContext">请求的上下文</param>
        /// <param name="values">表示不区分大小写的键/值对的集合，您可以在路由框架中的不同位置（例如，在定义路由的默认值时或在生成基于路由的 URL 时）使用该集合。</param>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return base.GetVirtualPath(requestContext, RemoveDomainTokens(values));
        }

        /// <summary>
        ///     获取路由数据
        /// </summary>
        /// <param name="requestContext">请求的上下文</param>
        /// <param name="values">表示不区分大小写的键/值对的集合，您可以在路由框架中的不同位置（例如，在定义路由的默认值时或在生成基于路由的 URL 时）使用该集合。</param>
        public DomainData GetDomainData(RequestContext requestContext, RouteValueDictionary values)
        {
            // 获得主机名
            var hostname = Domain;
            foreach (var pair in values) { hostname = hostname.Replace("{" + pair.Key + "}", pair.Value.ToString()); }

            // Return 域名数据
            return new DomainData { Protocol = "http", HostName = hostname, Fragment = "" };
        }

        /// <summary>
        ///     创建正则
        /// </summary>
        /// <param name="source">要转换的路径</param>
        private Regex CreateRegex(string source)
        {
            // 替换
            source = source.Replace("/", @"\/?");
            source = source.Replace(".", @"\.?");
            source = source.Replace("-", @"\-?");
            source = source.Replace("{", @"(?<");
            source = source.Replace("}", @">([a-zA-Z0-9_\-]*))");

            return new Regex("^" + source + "$");
        }

        private RouteValueDictionary RemoveDomainTokens(RouteValueDictionary values)
        {
            var tokenRegex = new Regex(@"({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?({[a-zA-Z0-9_]*})*-?\.?\/?");
            var tokenMatch = tokenRegex.Match(Domain);
            for (var i = 0; i < tokenMatch.Groups.Count; i++)
            {
                var group = tokenMatch.Groups[i];
                if (group.Success)
                {
                    var key = group.Value.Replace("{", "").Replace("}", "");
                    if (values.ContainsKey(key)) { values.Remove(key); }
                }
            }

            return values;
        }

        /// <summary>
        ///     获取路由值
        /// </summary>
        /// <param name="data">路由</param>
        /// <param name="match">正则匹配</param>
        /// <param name="regex">正则</param>
        private void GetRouteData(RouteData data, Match match, Regex regex)
        {
            // 匹配路径
            for (var i = 1; i < match.Groups.Count; i++)
            {
                var group = match.Groups[i];
                if (!group.Success) { continue; }
                var key = regex.GroupNameFromNumber(i);

                if (string.IsNullOrEmpty(key) || char.IsNumber(key, 0) || string.IsNullOrEmpty(group.Value)) { continue; }

                data.Values[key] = group.Value;
            }
        }
    }
}