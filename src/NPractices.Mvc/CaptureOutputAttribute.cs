using System;
using System.Web.Mvc;

namespace NPractices.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CaptureOutputAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string id = (filterContext.RouteData.Values["id"] ?? "index").ToString();
            string filename = filterContext.HttpContext.Server.MapPath(
                string.Format("~/output/{0}/{1}/{2}.html", controllerName, actionName, id));


            filterContext.HttpContext.Response.Filter = new CaptureOutputStream(filterContext.HttpContext.Response.Filter, filename);
        }
    }
}