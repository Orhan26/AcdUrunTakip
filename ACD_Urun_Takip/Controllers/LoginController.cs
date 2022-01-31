using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using ACD_Urun_Takip.Repository.Login;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ACD_Urun_Takip.Controllers
{
    public class LoginController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        [AllowAnonymous]
        public ActionResult Index(string returnurl)
        {
            if (returnurl != "/" || returnurl != null || returnurl != "" || !returnurl.ToLower().Contains("login"))
            {
                Session["returnurl"] = returnurl;
            }

            (string dbVersion, Version version) data = LoginRepository.GetDbVersion();
            ViewBag.Version = data.version.Major.ToString() + "." + data.version.Minor.ToString() + "." + data.version.Build.ToString();
            ViewBag.Version += "-" + data.dbVersion;

            if (ConfigurationManager.AppSettings["rememberme"] != null)
                ViewBag.Rememberme = ConfigurationManager.AppSettings["rememberme"] == "disable";
            else
                ViewBag.Rememberme = false;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [CustomFilter]
        public async Task<ActionResult> Index(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                Task<AjaxResult> result = LoginRepository.LoginAsync(loginModel);

                await Task.WhenAll(result);
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
            return Json(new AjaxResult { Success = false, Code = 3, Message = "Girilen bilgilere ait kullanıcı bulunmamaktadır.Lütfen bilgilerinizi kontrol ediniz." }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            var tmp = SecurityHelper.GetLoggedInUsers().FirstOrDefault(user => user.Username == HttpContext.User.Identity.Name);
            SecurityHelper.GetLoggedInUsers().Remove(tmp);
            return RedirectToAction("Index", "Login");
        }
    }
}