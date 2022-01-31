using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using ACD_Urun_Takip.Repository.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{

    [Authorize]
    //[ErrorHandle(View = "Error500")]
    public class DefineController : Controller
    {
        // GET: Define
        public ActionResult Index()
        {
            return View();
        }
        [TrackLoginsFilter]
        [LogElapsedTime]
        public ActionResult List(string tablename)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            DefineTable tbl = DefineRepository.GetDefineTbl(tablename, userId);
            return View(tbl);
        }
        /// <summary>
        /// Burası herhangi bir tanımlama tablosu listeleme işleminin dinamik olarak sayfa yüklendikten sonra yapıldığı yerdir.Listeleme yapılır
        /// </summary>
        /// <param name="tablename">Tablo Adı</param>
        /// <returns></returns>
        public PartialViewResult Tbody(string tablename)
        {
            DefineTable table = Task.Run(() => { return DefineRepository.GetDefineTblBody(tablename); }).Result;
            return PartialView(table);
        }


        /// <summary>
        /// Herhangi bir tanımlama tablosunda bulunan aramaların vs. değişikliklere göre dinamik olarak kayıtları getirmek için AJAX ile sorgu atılan method burasıdır.
        /// </summary>
        /// <param name="model">JustBody methodunun çağrıldığı ve bu işlemler için viewın AJAX sorgusunu attığı method olarak kullanılır.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxGetJsonData(JQueryDataTableModel model)
        {
            JQueryDataTable table = DefineRepository.GetDefineTblData(model);

            return Json(table, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Image tablosu ayrı bir tablodur ve tüm tanımlamalar içerisinde image içeren tüm tablolar bu tabloda tutulan image tipindeki verinin ID si ile beslenir.
        /// </summary>
        /// <param name="id">Image tablosuna ait id olarak kullanılır.</param>
        /// <returns></returns>
        [LogElapsedTime]
        public ActionResult GetImage(int id)
        {
            byte[] imageData = null;
            imageData = DefineRepository.GetImage(id);
            return (imageData != null) ? File(imageData, "image/png") : null;

        }

        /// <summary>
        /// Tanımlama tabloları içerisindeki yeni oluşturulacak bir kayıt için gerekli olan sütunları form içerisinde oluşturur.
        /// Yeni kayıt eklenirken ilk burası çağrılır ve modal içerisine yüklenir.
        /// Permission:Create
        /// </summary>
        /// <param name="tablename">Tablo adını parametre olarak alır.</param>
        /// <returns></returns>
        [LogElapsedTime]
        //[HasPermission(Models.Home.MenuPermission.Create)]
        public ActionResult Create(string tablename, string GuID)
        {
            List<SqlDataType> dt = DefineRepository.GetRowTypes(tablename, GuID);
            return View(dt);
        }

        /// <summary>
        /// Tanımlama tabloları için yeni bir kayıtın oluşturulduğu yerdir.Tablo adını ve bu kayıt için girilen verileri alır.
        /// </summary>
        /// <param name="fm">Form tarafından post edilen yeni kaydın verilerini generic olarak FormCollection tipinde alır</param>
        /// <param name="tablename">Ayrıca tablo adını parametre olarak alır.</param>
        /// <returns></returns>
        [LogElapsedTime]
        [HttpPost]
        [CustomFilter]
        public JsonResult Insert(FormCollection fm, string tablename)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = DefineRepository.SaveDefineTbl(fm, tablename, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update öncesi tek kaydın verilerini almak için kullanılmaktadır.
        /// </summary>
        /// <param name="model">Delete ile aynı parametreleri kullanmaktadır.</param>
        /// <returns></returns>
        [LogElapsedTime]
        [HttpPost]
        [CustomFilter]
        public JsonResult GetRow(DefineRowModel model)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            List<dynamic> result = DefineRepository.GetRow(model, userId);
            return Json(result[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Yüklenen image'i db ye kayıt eder.
        /// </summary>
        /// <param name="upload">Post edilen image</param>
        /// <returns></returns>
        [LogElapsedTime]
        [HttpPost]
        //[CustomFilter]
        public JsonResult UploadImage(HttpPostedFileBase upload)
        {
            AjaxResult result = DefineRepository.UploadImage(upload);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tanımlama tabloları içerisindeki güncelleme yapılacak kayıt için gerekli olan sütunları form içerisinde oluşturur.
        /// Permission:Update
        /// </summary>
        /// <param name="tablename">Tablo adını parametre olarak alır.</param>
        /// <returns></returns>
        [LogElapsedTime]
        //[HasPermission(Models.Home.MenuPermission.Update)]
        public ActionResult GetRowForUpdate(string tablename, string GuID)
        {
            List<SqlDataType> dt = DefineRepository.GetRowTypes(tablename, GuID);
            return View("Create", dt);
        }

        /// <summary>
        /// Tanımlama tabolarındaki update işlemini yapan method olarak kullanılmaktadır.
        /// </summary>
        /// <param name="fm">Yine insert için yapılan gibi FormCollection içerisinde değişiklik yapılan kaydın tüm verilerini getirir.</param>
        /// <param name="text">tablo adı ve Guid _ kullanılarak birleşik halde alınır ve parçalanır.</param>
        /// <returns></returns>
        [LogElapsedTime]
        [HttpPost]
        [CustomFilter]
        public JsonResult Update(FormCollection fm, string text)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = DefineRepository.UpdateDefineTbl(fm, text, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Toplu şekilde kayıt silmek için kullanılan method burasıdır.
        /// </summary>
        /// <param name="model">Tablo adını ve silinmesi gereken kayıtların GUID lerini parametre olarak alır</param>
        /// <returns></returns>
        [LogElapsedTime]
        [HttpPost]
        [CustomFilter]
        //[HasPermission(Models.Home.MenuPermission.Delete)]
        public JsonResult MultiDelete(DefineDeleteModel model)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = DefineRepository.DeleteAllDefineTbl(model, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Toplu Update yapılmasını sağlayan method burasıdır
        /// </summary>
        /// <param name="fm"></param>
        /// <returns></returns>
        [HttpPost]
        [LogElapsedTime]
        public JsonResult MultiUpdate(FormCollection fm)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = DefineRepository.UpdateSelectedRow(fm, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}