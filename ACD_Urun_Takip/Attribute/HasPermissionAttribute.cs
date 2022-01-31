using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using ACD_Urun_Takip.Repository.Attribute;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Attribute
{
    /// <summary>
    /// Kullanıcıların user gruplarına göre izinlerinin yönetilmesi
    /// Permission izin verilmediğinde JsonResult türünde dönüş yapılmaktadır. Hata:ErrorMessage, Success:false!
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        private MenuPermission Action { get; set; }
        private string UserName { get; set; }
        public HasPermissionAttribute(MenuPermission permission)
        {
            this.Action = permission;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool useAuthorization = false;
            this.UserName = filterContext.HttpContext.User.Identity.Name;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string userGroupName = filterContext.HttpContext.User.Identity.Name.Split('_')[4];
                if (string.IsNullOrEmpty(userGroupName))
                    useAuthorization = false;

                useAuthorization = IsPermissionInUserGroup();
            }
            if (!useAuthorization)
            {

                filterContext.Result = new JsonResult { Data = new AjaxResult() { Success = false, Message = $"{EnumHelper<MenuPermission>.GetDisplayValue(this.Action)} işlemi için izniniz bulunmamaktadır." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            base.OnAuthorization(filterContext);
        }
        public bool IsPermissionInUserGroup()
        {
            bool _retVal = false;
            try
            {
                List<AllMenu> allMenus = AttributeRepository.GetMenuWithPermission(this.UserName);
                //Remove Culture -- /tr/
                var urlReferrerArr = HttpContext.Current.Request.UrlReferrer.LocalPath.Split('/').Skip(2);
                var urlReferrer = string.Join("/ ", urlReferrerArr);
                var queryArr = HttpContext.Current.Request.UrlReferrer.Query.Split('=');
                //PivotTablolar menüde olmadığı için ilgili raporun izini yürütülmektedir
                //http://localhost:58548/tr/Report/PivotReport?tablename=Production_Report_23012017_Weekly_18092019&columnX=Week&func=Count&columnZ=Wrs_Name&pivot=48
                if (urlReferrer.Contains("PivotReport"))
                {
                    urlReferrer = "Report/List";
                    string[] newQueryArr = HttpContext.Current.Request.UrlReferrer.Query.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var query in newQueryArr)
                    {
                        if (!query.StartsWith("?tablename"))
                            continue;
                        queryArr = query.Split('=');

                    }
                }
                foreach (var query in queryArr)
                {
                    if (query.StartsWith("?"))
                        continue;

                    string newQuery = string.Concat(urlReferrer, "/", query);

                    //Remove whiteSpace
                    AllMenu menu = allMenus.Where(x => String.Concat(x.Path.Where(c => !Char.IsWhiteSpace(c))) == String.Concat(newQuery.Where(c => !Char.IsWhiteSpace(c)))).ToArray().FirstOrDefault();
                   
                    //Home/CreateUser/ durumlarda db'den Home/CreateUser/- geldiği için kontrol eklendi
                    if (menu == null)
                    {
                        if (newQuery.Split('/').Length == 3 && newQuery.EndsWith("/"))
                        {
                            newQuery += "-";
                            menu = allMenus.Where(x => String.Concat(x.Path.Where(c => !Char.IsWhiteSpace(c))) == String.Concat(newQuery.Where(c => !Char.IsWhiteSpace(c)))).ToArray().FirstOrDefault();
                        }

                    }

                    if (menu != null)
                    {
                        switch (this.Action)
                        {
                            case MenuPermission.Create:
                                _retVal = menu.IsCreate ? true : false;
                                break;
                            case MenuPermission.Update:
                                _retVal = menu.IsUpdate ? true : false;
                                break;
                            case MenuPermission.Delete:
                                _retVal = menu.IsDelete ? true : false;
                                break;
                        }

                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return _retVal;
        }
      

    }
}