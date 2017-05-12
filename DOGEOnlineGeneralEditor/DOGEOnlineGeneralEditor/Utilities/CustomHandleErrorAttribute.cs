using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOGEOnlineGeneralEditor.Utilities
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;

            string currentController = (string)filterContext.RouteData.Values["controller"];
            string currentActionName = (string)filterContext.RouteData.Values["action"];

            string viewName = "Error";

            if (ex is CustomProjectNotFoundException)
            {
                viewName = "ProjectNotFoundError";
            }
            else if (ex is CustomFileNotFoundException)
            {
                viewName = "FileNotFoundError";
            }
            else if (ex is UnauthorizedAccessToProjectException)
            {
                viewName = "UnauthorizedAccessError";
            }

            HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, currentController, currentActionName);
            ViewResult result = new ViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };

            filterContext.Result = result;
            filterContext.ExceptionHandled = true;

            base.OnException(filterContext);
        }
    }
}