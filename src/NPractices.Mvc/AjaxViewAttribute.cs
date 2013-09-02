using System;
using System.Web.Mvc;

namespace NPractices.Mvc
{
    /// <summary>
    /// return partial view if ajax request on action
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AjaxViewAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var res = filterContext.Result as ViewResult;
            if (res == null)
                return;

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                filterContext.Result = new PartialViewResult
                                           {
                                               ViewName = res.ViewName,
                                               ViewData = res.ViewData,
                                               TempData = res.TempData
                                           };
        }
    }
}