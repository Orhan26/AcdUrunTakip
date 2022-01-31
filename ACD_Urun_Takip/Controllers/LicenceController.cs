using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACD_Urun_Takip.Models;
using System.Web.Mvc;
using System.Data.SqlClient;
using ACD_Urun_Takip.Repository.License;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Program;
using ACD_Urun_Takip.Repository.Modul;

namespace ACD_Urun_Takip.Controllers
{
    public class LicenceController : Controller
    {
        // GET: Lisans
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<LicenceInfo> licenceList = LicenceRepository.AllLicenses();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in licenceList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' class='thfirst-check' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' type='checkbox' value=" + item.LicenceId.ToString() + " />");
                colums.Add("ProgramName", item.Program.ProgramName);
                colums.Add("CompanyName", item.Company.CompanyName);
                colums.Add("ModuleName", item.Module.ModuleName);
                colums.Add("LicenceKey", item.LicenceKey.Length > 20? item.LicenceKey.Substring(0,20)+"...": item.LicenceKey);
                columsList.Add(colums);
            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Modull(int Program)
        {
            var result = LicenceRepository.Modul(Program);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddLicenceView()
        {
            ListInformation list = new ListInformation();
            list.Companies = CompanyRepository.Companys();
            list.ProgramInfos= ProgramRepository.AllPrograms();
            list.LicenceInfo = null;
            return PartialView("AddLicence",list);
        }
        [HttpPost]
        public string AddLicence(LicenceInfo Add)
        {
            var result = LicenceRepository.AddLicense(Add);
            return result;
        }
        [HttpPost]
        public string DeleteLicence(int[] LicenceId)
        {
            var result = LicenceRepository.DeleteLicense(LicenceId);
            return result;
        }
        [HttpGet]
        public ActionResult GetLicence(int LicenceId)
        {
            ListInformation list = new ListInformation();
            list.Companies = CompanyRepository.Companys();
            list.ProgramInfos = ProgramRepository.AllPrograms();
            list.LicenceInfo = LicenceRepository.GetLicense(LicenceId);
            list.Modules = ModulRepository.Modules().Where(o => o.Program.ProgramId == list.LicenceInfo.Program.ProgramId).ToList();
            return PartialView("AddLicence", list);
        }
        [HttpPost]
        public string UpdateLicence(LicenceInfo update)
        {
            var result = LicenceRepository.UpdateLicanse(update);
            return result;
        }
    }
}