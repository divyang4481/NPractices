using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NPractices.Mvc
{
    /// <summary>
    /// An implementation of Error handler for action: use another result to handle the error
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class HandleErrorWithResultAttribute : FilterAttribute, IExceptionFilter
    {
        private readonly Type _resultType = typeof(ActionResult);
        private Type _exceptionType = typeof(Exception);

        public HandleErrorWithResultAttribute(Type resultType)
        {
            if (resultType == null)
                throw new ArgumentNullException("resultType");

            if (!typeof(ActionResult).IsAssignableFrom(resultType))
                throw new ArgumentException("resultType is not a type of ActionResult");

            _resultType = resultType;
        }

        /// <summary>
        /// only catch the exception type
        /// </summary>
        public Type ExceptionType
        {
            get { return _exceptionType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!typeof(Exception).IsAssignableFrom(value))
                    throw new ArgumentException("value is not a type of exception");

                _exceptionType = value;
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");
            if (filterContext.IsChildAction)
                return;
            if (filterContext.ExceptionHandled)
                return;

            Exception exception = filterContext.Exception;

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, exception).GetHttpCode() != 500)
                return;

            if (!ExceptionType.IsInstanceOfType(exception))
                return;

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            string message = null;
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                message = (from e in filterContext.Controller.ViewData.ModelState
                           where e.Value.Errors.Count > 0
                           select e.Value.Errors[0].ErrorMessage).FirstOrDefault();
            }
            else
            {
                ArgumentException argException = exception as ArgumentException;
                if (argException != null && argException.ParamName != null)
                    filterContext.Controller.ViewData.ModelState.AddModelError(argException.ParamName,
                                                                               argException.Message);
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = exception.Message;
                filterContext.Controller.ViewData.ModelState.AddModelError("", exception.Message);
            }

            #region ViewResult

            if (_resultType == typeof(ViewResult))
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new PartialViewResult
                                               {
                                                   ViewName = null,
                                                   ViewData = filterContext.Controller.ViewData,
                                                   TempData = filterContext.Controller.TempData
                                               };
                else
                    filterContext.Result = new ViewResult
                                               {
                                                   ViewName = null,
                                                   MasterName = null,
                                                   ViewData = filterContext.Controller.ViewData,
                                                   TempData = filterContext.Controller.TempData
                                               };
            }

            #endregion

            #region ContentResult

            if (_resultType == typeof(ContentResult))
            {
                filterContext.Result = new ContentResult
                                           {
                                               ContentType = null,
                                               ContentEncoding = null,
                                               Content = message
                                           };
            }

            #endregion

            #region JsonResult

            if (_resultType == typeof(JsonResult))
            {
                filterContext.Result = new JsonResult
                                           {
                                               Data = new { Error = message }
                                           };
            }

            #endregion

            #region HttpStatusCodeResult

            if (_resultType == typeof(HttpStatusCodeResult))
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError, filterContext.HttpContext.Server.UrlEncode(message));
            }

            #endregion
        }
    }
}