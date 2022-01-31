using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Attribute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ACD_Urun_Takip.Attribute
{
    public class TrackLoginsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            string param = HttpContext.Current.Request.QueryString.ToString();

            if (SecurityHelper.GetLoggedInUsers().Any(x => x.Username == HttpContext.Current.User.Identity.Name))
            {
                if (!SecurityHelper.GetLoggedInUsers().FirstOrDefault(x => x.Username == HttpContext.Current.User.Identity.Name).Active)
                {
                    FormsAuthentication.SignOut();
                    var tmp = SecurityHelper.GetLoggedInUsers().FirstOrDefault(x => !x.Active);
                    SecurityHelper.GetLoggedInUsers().Remove(tmp);
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
                else
                {
                    List<LoggedIn> loggedInUsers = SecurityHelper.GetLoggedInUsers();

                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        if (loggedInUsers.Any(x => x.Username == HttpContext.Current.User.Identity.Name))
                        {
                            loggedInUsers.FirstOrDefault(x => x.Username == HttpContext.Current.User.Identity.Name).Date = DateTime.Now;
                            loggedInUsers.FirstOrDefault(x => x.Username == HttpContext.Current.User.Identity.Name).Page = AttributeRepository.GetPage(controller, action, param);

                        }
                        else
                        {
                            loggedInUsers.Add(new LoggedIn
                            {
                                Date = DateTime.Now,
                                Page = AttributeRepository.GetPage(controller, action, param),
                                Username = HttpContext.Current.User.Identity.Name,
                                Active = true
                            });
                        }

                    }
                    else
                    {
                        if (!HttpContext.Current.Response.IsRequestBeingRedirected)
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                            filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                        }
                    }

                    // remove users where time exceeds session timeout
                    var keys = loggedInUsers.Where(u => DateTime.Now.Subtract(u.Date).Minutes > HttpContext.Current.Session.Timeout);
                    foreach (var key in keys)
                    {
                        loggedInUsers.Remove(key);
                    }
                }
            }
            else
            {
                List<LoggedIn> loggedInUsers = SecurityHelper.GetLoggedInUsers();
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (loggedInUsers.Any(x => x.Username == HttpContext.Current.User.Identity.Name))
                    {
                        loggedInUsers.FirstOrDefault(x => x.Username == HttpContext.Current.User.Identity.Name).Date = DateTime.Now;
                        loggedInUsers.FirstOrDefault(x => x.Username == HttpContext.Current.User.Identity.Name).Page = AttributeRepository.GetPage(controller, action, param);

                    }
                    else
                    {
                        loggedInUsers.Add(new LoggedIn
                        {
                            Date = DateTime.Now,
                            Page = AttributeRepository.GetPage(controller, action, param),
                            Username = HttpContext.Current.User.Identity.Name,
                            Active = true
                        });
                    }
                }

                // remove users where time exceeds session timeout
                var keys = loggedInUsers.Where(u => DateTime.Now.Subtract(u.Date).Minutes >
                           HttpContext.Current.Session.Timeout);
                foreach (var key in keys)
                {
                    loggedInUsers.Remove(key);
                }
            }
        }


    }
}