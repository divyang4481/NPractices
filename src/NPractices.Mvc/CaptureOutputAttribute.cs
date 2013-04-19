using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace NPractices.Mvc
{
    /// <summary>
    /// capture the action result rendered content to a file
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CaptureOutputAttribute : ActionFilterAttribute
    {
        private static readonly Regex RxRouteValue = new Regex(@"\{(\w+)\}", RegexOptions.Compiled);
        private string _outputFile = "~/output/{controller}/{action}/{id}.html";

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            string filename = filterContext.HttpContext.Server.MapPath(GetOutputFilePath(filterContext.RouteData.Values));

            filterContext.HttpContext.Response.Filter = new CaptureOutputStream(filterContext.HttpContext.Response.Filter, filename);
        }

        /// <summary>
        /// output path:
        /// e.g. ~/output/{controller}/{action}/{id}.html
        /// </summary>
        public string OutputFile
        {
            get { return _outputFile; }
            set { _outputFile = value; }
        }

        private string GetOutputFilePath(RouteValueDictionary routeValues)
        {
            return RxRouteValue.Replace(OutputFile, m => (string)routeValues[m.Groups[1].Value]);
        }
    }
}