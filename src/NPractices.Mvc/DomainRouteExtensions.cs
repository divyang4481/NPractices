using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace NPractices.Mvc
{
    /// <summary>
    /// extensions for DomainRoute
    /// </summary>
    public static class DomainRouteExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this RouteCollection routes, string name, string domain, string url)
        {
            return MapDomainRoute(routes, name, domain, url, null /* defaults */, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this RouteCollection routes, string name, string domain, string url, object defaults)
        {
            return MapDomainRoute(routes, name, domain, url, defaults, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this RouteCollection routes, string name, string domain, string url, object defaults, object constraints)
        {
            return MapDomainRoute(routes, name, domain, url, defaults, constraints, null /* namespaces */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this RouteCollection routes, string name, string domain, string url, string[] namespaces)
        {
            return MapDomainRoute(routes, name, domain, url, null /* defaults */, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this RouteCollection routes, string name, string domain, string url, object defaults, string[] namespaces)
        {
            return MapDomainRoute(routes, name, domain, url, defaults, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this RouteCollection routes, string name, string domain, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            Route route = new DomainRoute(url, new MvcRouteHandler())
            {
                Domain = domain,
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        }

        #region Area

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
    Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this AreaRegistrationContext context, string name, string domain, string url)
        {
            return MapDomainRoute(context, name, domain, url, null /* defaults */, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this AreaRegistrationContext context, string name, string domain, string url, object defaults)
        {
            return MapDomainRoute(context, name, domain, url, defaults, (object)null /* constraints */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this AreaRegistrationContext context, string name, string domain, string url, object defaults, object constraints)
        {
            return MapDomainRoute(context, name, domain, url, defaults, constraints, null /* namespaces */);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this AreaRegistrationContext context, string name, string domain, string url, string[] namespaces)
        {
            return MapDomainRoute(context, name, domain, url, null /* defaults */, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this AreaRegistrationContext context, string name, string domain, string url, object defaults, string[] namespaces)
        {
            return MapDomainRoute(context, name, domain, url, defaults, null /* constraints */, namespaces);
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapDomainRoute(this AreaRegistrationContext context, string name, string domain, string url, object defaults, object constraints, string[] namespaces)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Route route = MapDomainRoute(context.Routes, name, domain, url, defaults, constraints,
                ((namespaces != null) && (namespaces.Length > 0)) ? namespaces : context.Namespaces.ToArray());

            route.DataTokens["area"] = context.AreaName;
            route.DataTokens["UseNamespaceFallback"] = false;

            return route;
        }

        #endregion
    }
}