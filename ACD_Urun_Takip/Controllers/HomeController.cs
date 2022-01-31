using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using ACD_Urun_Takip.Repository.Home;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{
    [Authorize]
    //[ErrorHandle(View = "Error500")]
    public class HomeController : Controller
    {
        [TrackLoginsFilter]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Önemli! Language.json dosyasının güncel olması için dil tablosunda yapılan değişiklik sonrası burası çalıştırılmalıdır.
        /// </summary>
        /// <returns></returns>
        public JsonResult CreateLanguage()
        {
            string language = HomeRepository.CreateLanguage();
            return Json(language, JsonRequestBehavior.AllowGet);
        }

        [LogElapsedTime]
        public string SaveGlobalLog(GlobalLogModel model)
        {
            string result = HomeRepository.SaveGlobalLog(model);
            return result;
        }

        /// <summary>
        /// Error500 Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Error500()
        {
            return View("~/Views/Shared/Error500.cshtml");
        }

        /// <summary>
        /// Erişim Engellendi
        /// </summary>
        /// <returns></returns>
        public ViewResult AccessDenied()
        {
            return View();
        }

        //TODO:!!Menu REfresh


        [AllowAnonymous]
        [HttpPost]
        [CustomFilter]
        public JsonResult ReturnMessage(FormCollection fm)
        {
            return Json(new AjaxResult { Success = true, Message = DbOperation.ExecuteScalar(fm) }, JsonRequestBehavior.AllowGet);
        }
    }
}