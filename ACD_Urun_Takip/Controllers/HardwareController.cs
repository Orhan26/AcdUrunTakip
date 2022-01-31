using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Hardware;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{
    public class HardwareController : Controller
    {
        // GET: Donanim
        //@Url.Action('Download','Hardware', new { hardwareId = "+item.HardwareId+" },null)
        public string name;
        public string dpath;
        public string msj;
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<HardwareInfo> hardwareList = HardwareRepository.AllHardWares();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in hardwareList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' class='thfirst-check' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' type='checkbox' value=" + item.HardwareId.ToString() + " />");
                colums.Add("HardwareName", item.HardwareName);
                colums.Add("HardwareType", item.HardwareType);
                colums.Add("DocumentName", item.DocumentName);
                if(item.iconHide == false)
                {
                 colums.Add("icon", "<a href=http://localhost:50542/tr/Hardware/Download?hardwareId=" + item.HardwareId+"><i class='fas fa-download' style='font-size:20px'></i></a>");
                }
                else
                {
                    colums.Add("icon", "");
                }
                columsList.Add(colums);
            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddHardwareView()
        {
            HardwareInfo hardwareInfo = new HardwareInfo();
            return PartialView(hardwareInfo);
        }
        [HttpPost]
        public string Upload(string hardwareName, string hardwareType)
        {
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 1)
                    {
                        var fi = new FileInfo(file.FileName);
                        var fileName = Path.GetFileName(file.FileName);
                        string path = Server.MapPath("~/Donanım Dosyaları/" + hardwareName);
                        var path2 = Path.Combine(Server.MapPath("~/Donanım Dosyaları/" + hardwareName + "/"), fileName);
                        Directory.CreateDirectory(path);
                        file.SaveAs(path2);

                        name = fileName;
                        dpath = path; 
                    }
                }
                HardwareInfo hardwareInfo = new HardwareInfo();
                hardwareInfo.HardwareName = hardwareName;
                hardwareInfo.HardwareType = hardwareType;
                hardwareInfo.DocumentName = name;
                hardwareInfo.DocumentPath = dpath;
                var result = HardwareRepository.AddHardware(hardwareInfo);
                return result;
            }
        }
        [HttpGet]
        public FileResult Download(int hardwareId)
        { 
            var result = HardwareRepository.GetHardware(hardwareId);

            string path = AppDomain.CurrentDomain.BaseDirectory + "Donanım Dosyaları\\" + result.HardwareName + "\\";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + result.DocumentName);
            string fileName = result.DocumentName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public string DeleteHardware(int[] hardwareId)
        {
            var result = HardwareRepository.DeleteHardware(hardwareId);
            return result;
        }
        [HttpGet]
        public ActionResult GetHardware(int hardwareId)
        {
            var result = HardwareRepository.GetHardware(hardwareId);
            return PartialView("AddHardwareView", result);
        }
        public string UpdateHardware(int hardwareId, string hardwareName,string hardwareType)
        {
            HardwareInfo gelen = new HardwareInfo();
            gelen = HardwareRepository.GetHardware(hardwareId);
            string fileName,path2;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength == 0)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "HardwareProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 5);
                    komut.Parameters.AddWithValue("@hardwareId", hardwareId);
                    komut.Parameters.AddWithValue("@hardwareName", hardwareName);
                    komut.Parameters.AddWithValue("@hardwareType", hardwareType);
                    komut.Parameters.AddWithValue("@userId", System.Web.HttpContext.Current.User.Identity.Name.Split('_')[1]);
                    conn.Open();
                    komut.ExecuteNonQuery();
                    conn.Close();
                    msj = "Düzenleme kaydedildi";
                }
                else
                {
                    if (gelen.DocumentPath == "")
                    {
                        var fi = new FileInfo(file.FileName);
                        fileName = Path.GetFileName(file.FileName);
                        string yolu = Server.MapPath("~/Donanım Dosyaları/" + hardwareName);
                        path2 = Path.Combine(Server.MapPath("~/Donanım Dosyaları/" + hardwareName + "/"), fileName);
                        Directory.CreateDirectory(yolu);
                        file.SaveAs(path2);
                    }
                    else
                    {
                        string file_name = gelen.HardwareName;
                        string path = gelen.DocumentPath;
                        FileInfo files = new FileInfo(path);
                        if (files.Exists)
                        {
                            files.Delete();
                        }
                        var fi = new FileInfo(file.FileName);
                        fileName = Path.GetFileName(file.FileName);
                        string yolu = Server.MapPath("~/Donanım Dosyaları/" + hardwareName);
                        path2 = Path.Combine(Server.MapPath("~/Donanım Dosyaları/" + hardwareName + "/"), fileName);
                        Directory.CreateDirectory(yolu);
                        file.SaveAs(path2);
                    }
                    gelen.DocumentName = fileName;
                    gelen.DocumentPath = path2;
                    gelen.HardwareName = hardwareName;
                    gelen.HardwareType = hardwareType;
                    gelen.HardwareId = hardwareId;
                    msj = HardwareRepository.UpdateHardware(gelen);
                }
            }
            return msj; 
        }
    }
}