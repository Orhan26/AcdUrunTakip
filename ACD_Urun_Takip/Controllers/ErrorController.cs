using ACD_Urun_Takip.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{
    [Authorize]
    //[ErrorHandle(View = "Error500")]
    // TODO:Kullanıcıların göreceği Error Sayfaları Yaratılmalı mı?
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}