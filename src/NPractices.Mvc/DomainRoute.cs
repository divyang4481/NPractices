using System;
using System.Web;
using System.Web.Routing;

namespace NPractices.Mvc
{
    /// <summary>
    /// route domain url to area:
    /// e.g. http://www.domain.com => /{area}/...
    /// </summary>
    public class DomainRoute : Route
    {
        public DomainRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        public DomainRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        public DomainRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public DomainRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }

        public string Domain { get; set; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // Request information
            string requestDomain = httpContext.Request.Headers["host"];
            if (!string.IsNullOrEmpty(requestDomain))
            {
                if (requestDomain.IndexOf(":") > 0)
                    requestDomain = requestDomain.Substring(0, requestDomain.IndexOf(":"));
            }
            else
            {
                requestDomain = httpContext.Request.Url.Host;
            }

            if (!requestDomain.Equals(Domain, StringComparison.OrdinalIgnoreCase))
                return null;

            var result = base.GetRouteData(httpContext);

            return result;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            // Checks if the area to generate the route against is this same as the domain
            // If so we remove the area value so it won't be added to the URL as a query parameter
            if (values != null && values.ContainsKey("Area"))
            {
                if (values["Area"].ToString().Equals(Domain, StringComparison.OrdinalIgnoreCase))
                {
                    values.Remove("Area");
                    return base.GetVirtualPath(requestContext, values);
                }
            }

            return null;
        }
    }
}
