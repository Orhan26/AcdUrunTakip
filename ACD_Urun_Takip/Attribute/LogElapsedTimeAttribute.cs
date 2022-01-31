using ACD_Urun_Takip.Repository.Attribute;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Attribute
{
    public class LogElapsedTimeAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            filterContext.HttpContext.Items["timer"] = Stopwatch.StartNew();
            base.OnActionExecuting(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Stopwatch sw = filterContext.HttpContext.Items["timer"] as Stopwatch;
            float f = sw.ElapsedMilliseconds;
            AttributeRepository.SaveLogElapsedTime(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName, filterContext.HttpContext.Request.QueryString.ToString(), f, filterContext.HttpContext.User.Identity.Name.Split('_')[1]);
            base.OnActionExecuted(filterContext);
        }

    }
}