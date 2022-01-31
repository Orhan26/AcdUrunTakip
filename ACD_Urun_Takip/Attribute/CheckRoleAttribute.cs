using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Attribute
{
    [AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class CheckRoleAttribute : AuthorizeAttribute
    {
        public string[] AllowedRoles { get; set; }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool Match = false;
            string userName = filterContext.HttpContext.User.Identity.Name;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (AllowedRoles.Count() > 0)
                {
                    string usr_group = filterContext.HttpContext.User.Identity.Name.Split('_')[4];

                    if (AllowedRoles.Contains(usr_group))
                    {
                        Match = true;
                    }
                }
            }
            if (!Match)
            {
                filterContext.Result = new ViewResult { ViewName = "AccessDenied" };
            }

            base.OnAuthorization(filterContext);

        }
    }
}