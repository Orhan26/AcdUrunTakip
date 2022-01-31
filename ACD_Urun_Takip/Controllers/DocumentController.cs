using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Document;
using Syncfusion.Office;


namespace ACD_Urun_Takip.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Dokuman
        public string name;
        public string dpath;
        public string sonuc;
        SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public ActionResult Index()
        {
            //var result = DocumentRepository.GetDocumentInformation();
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<DocumentInfo> documentList = DocumentRepository.GetDocumentInformation();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in documentList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' class='thfirst-check' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' type='checkbox' value=" + item.DocumentId+" />");
                colums.Add("CompanyName", item.Company.CompanyName);
                colums.Add("DocumentName", item.DocumentName);
                colums.Add("Version", item.Version);
                colums.Add("Date", item.Date.ToString());
                colums.Add("Download", "<a href=http://localhost:50542/tr/Document/Download?dkid=" + item.DocumentId + "><i class='fas fa-download' style='font-size:15px;margin-left:5px'></i></a>");
                if (item.IsPreview)
                {
                    colums.Add("Preview", "<a href='#'><i onclick='preview("+item.DocumentId+")' class='fas fa-file-archive' style='font-size:15px;margin-left:20px'></i></a>");
                }
                else
                {
                    colums.Add("Preview", "");
                }
                columsList.Add(colums);

            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddDocument()
        {
            ListInformation list = new ListInformation();
            list.Companies = CompanyRepository.Companys();
            list.DocumentInfo = null;
            return PartialView(list);
        }
        [HttpPost]
        public string DeleteDocument(int[] id)
        {
            var result = DocumentRepository.DocumentDelete(id);
            return result;
        }
        [HttpPost]
        public JsonResult Upload(int companyId, string version)
        {

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength == 0)
                {
                    sonuc = "Dosya seçimi yapılmadı";
                }
                else
                {
                    string companyName = CompanyRepository.GetCompany(companyId).CompanyName;

                    var fi = new FileInfo(file.FileName);
                    var fileName = Path.GetFileName(file.FileName);
                    string path = Server.MapPath("~/Dosyalar/" + companyName);

                    var path2 = Path.Combine(Server.MapPath("~/Dosyalar/" + companyName + "/"), fileName);
                    Directory.CreateDirectory(path);

                    file.SaveAs(path2);

                    name = fileName;
                    dpath = path;
                    DocumentRepository.AddDocument(companyId, name, dpath,version);
                    sonuc = "Başarıyla kaydedildi";
                }
            }
            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }
        
        public FileResult Download(int dkid)
        {
            DocumentInfo documentInfo = new DocumentInfo();
            documentInfo = DocumentRepository.DosyaGetir(dkid);/*dokuman.DosyaGetir(dkid);*/
            string companyName = CompanyRepository.GetCompany(int.Parse(documentInfo.Company.CompanyId.ToString())).CompanyName;

            string path = AppDomain.CurrentDomain.BaseDirectory + "Dosyalar\\" + companyName + "\\"; 
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + documentInfo.DocumentName);
            string fileName = documentInfo.DocumentName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public ActionResult Preview(int dkid)
        {
            DocumentInfo documentInfo = new DocumentInfo();
            documentInfo = DocumentRepository.DosyaGetir(dkid);
            string companyName = CompanyRepository.GetCompany(int.Parse(documentInfo.Company.CompanyId.ToString())).CompanyName;

            string filename = documentInfo.DocumentName;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Dosyalar\\" + companyName + "\\" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            if (contentType == "application/pdf")
            {
                return File(filepath, "application/pdf");
            }
            else if (contentType == "text/plain")
            {
                return File(filepath, "text/plain");
            }
            else if (contentType == "application/vnd.openxmlformats-officedocument.presentationml.presentation")
            {
                return RedirectToAction("Index");
            }
            else if (contentType == "image/jpeg")
            {
                return File(filepath, "image/jpg");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
      
        [HttpGet]
        public ActionResult GetDocument(int id)
        {
            ListInformation list = new ListInformation();
            list.Companies = CompanyRepository.Companys();
            list.DocumentInfo = DocumentRepository.DosyaGetir(id);
            return PartialView("AddDocument",list); 
        }
        [HttpPost]
        public ActionResult UpdateDocument(int id,string version)
        {
            DocumentInfo gelen = new DocumentInfo();
            gelen = DocumentRepository.DosyaGetir(id);


            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength == 0)
                {
                    conn.Open();
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "update Dokumanlar set Tarih=GETDATE(),VersionNo=@version where DokumanID=@documentId";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.Text;
                    komut.Parameters.AddWithValue("@documentId", id);
                    komut.Parameters.AddWithValue("@version",version);
                    komut.ExecuteNonQuery();
                    conn.Close();
                    sonuc = "Düzenleme kaydedildi...";
                }
                else
                {
                    string file_name = gelen.DocumentName;
                    string path = gelen.DocumentPath + "/" + file_name;
                    FileInfo files = new FileInfo(path);
                    if (files.Exists)
                    {
                        files.Delete();
                    }
                    //string companyName = CompanyRepository.GetCompany(companyId).CompanyName;

                    var fi = new FileInfo(file.FileName);
                    var fileName = Path.GetFileName(file.FileName);
                    string yolu = Server.MapPath("~/Dosyalar/" + gelen.Company.CompanyName);

                    var path2 = Path.Combine(Server.MapPath("~/Dosyalar/" + gelen.Company.CompanyName + "/"), fileName);
                    Directory.CreateDirectory(yolu);

                    file.SaveAs(path2);
                    name = fileName;
                    dpath = yolu;

                    DocumentRepository.UpdateDocument(id, name, dpath,version);
                }
            }
            return Json(new { sonuc = "Düzenleme kaydedildi" }, JsonRequestBehavior.AllowGet);
        }
    }
}