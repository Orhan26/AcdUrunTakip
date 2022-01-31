using ACD_Urun_Takip.Attribute;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandleAttribute());
            filters.Add(new LocalizationAttribute("tr-TR"), 0);
        }
    }
}
