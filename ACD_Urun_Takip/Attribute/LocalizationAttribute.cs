using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Attribute
{
    public class LocalizationAttribute : ActionFilterAttribute
    {

        private string _DefaultLanguage = "tr";

        public LocalizationAttribute(string defaultLanguage)
        {
            _DefaultLanguage = defaultLanguage;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string lang = (string)filterContext.RouteData.Values["lang"] ?? _DefaultLanguage;
            if (lang != _DefaultLanguage)
            {
                try
                {
                    
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                    var currentUICulture = Thread.CurrentThread.CurrentUICulture.ToString();

                }
                catch (Exception ex)
                {
                    throw new NotSupportedException(String.Format("ERROR: Invalid language code '{0}'.Exception Error '{1}'", lang, ex.Message));
                }
            }
        }

    }
}