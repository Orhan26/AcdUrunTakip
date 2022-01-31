using ACD_Urun_Takip.Controllers;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Attribute
{
    public class ErrorHandleAttribute : HandleErrorAttribute
{
       public override void OnException(ExceptionContext filterContext)
      {
           var exceptionHandled = filterContext.ExceptionHandled;

          base.OnException(filterContext);
          if (!exceptionHandled && filterContext.ExceptionHandled)
            {
                int userId = Convert.ToInt32(filterContext.HttpContext.User.Identity.Name.Split('_')[1]);
            string table = filterContext.HttpContext.Request["tablename"] ?? filterContext.HttpContext.Request["reportname"];
            string param = filterContext.HttpContext.Request.QueryString.ToString();
            GlobalLogModel logModel = new GlobalLogModel
            {
               Description = filterContext.Exception.ToString(),
                ErrorType = filterContext.Exception.GetType().ToString(),
                Method = string.Join("/", filterContext.RouteData.Values.Select(a => a.Key + ":" + a.Value).ToArray()),
                Parameters = (table == null ? "" : "Table ->") + table + "&& params->" + param,
                UserId = userId
            };
           if (!filterContext.HttpContext.IsCustomErrorEnabled)
            {

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Clear();
                    var result = DependencyResolver.Current.GetService<HomeController>().SaveGlobalLog(logModel);
                    string cont = filterContext.RouteData.GetRequiredString("controller");
                    IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                    IController controller = factory.CreateController(filterContext.RequestContext, cont);
                    ControllerContext controllerContext = new ControllerContext(filterContext.RequestContext, (ControllerBase)controller);
                    JsonResult jsonResult = new JsonResult();
                    UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                   jsonResult.Data = new AjaxResult { Success = false, Message = "Beklenmedik Bir Hata Oluştu." + (result == "OK" ? "<br/>Bu Hata Kayıt Edilmiştir." : ""), Help = url.Action("Solution", "Error", new { err = filterContext.HttpContext.Error.GetType().ToString() }).ToString() };
                   jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    jsonResult.ExecuteResult(controllerContext);
                   filterContext.HttpContext.Response.End();
                    filterContext.ExceptionHandled = true;
                    return;
               }
                else
               {
                    var result = DependencyResolver.Current.GetService<HomeController>().SaveGlobalLog(logModel);
               }


            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }
            if (filterContext.ExceptionHandled)
            {
                return;
           }
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            filterContext.Result = new ViewResult
            {
                ViewName = View,
                MasterName = Master,
               ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;


           base.OnException(filterContext);
        }

   }
  }
}