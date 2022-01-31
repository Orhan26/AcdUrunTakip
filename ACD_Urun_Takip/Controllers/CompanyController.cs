using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.AuditRecord;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.CompanyGroup;
using ACD_Urun_Takip.Repository.Group;

namespace ACD_Urun_Takip.Controllers
{
    public class DataTableColums
    {
        public string chk;
        public string CompanyName;
        public string Mail;
        public string Phone;
        public string Adress;
        public string Authorized;
        public string AuthorizedPhone;
        public string AuthorizedMail;
    }
    public class CompanyController : Controller
    {
        // GET: Sirket
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public  JsonResult JsonModel()
        {
            List<CompanyInformation> companyList = CompanyRepository.Companys();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in companyList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' class='thfirst-check' type='checkbox' value=" + item.CompanyId.ToString() + " />");
                colums.Add("Phone", item.Phone);
                colums.Add("CompanyName", item.CompanyName);
                colums.Add("Authorized", item.Authorized);
                colums.Add("AuthorizedMail", item.AuthorizedMail);
                colums.Add("AuthorizedPhone", item.AuthorizedPhone);
                colums.Add("Adress", item.Address);
                colums.Add("Mail", item.Mail);
                columsList.Add(colums);

            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCompanyView()
        {
            CompanyInformation s = new CompanyInformation();
            return PartialView(s);
        }
        public string AddCompany(CompanyInformation Record)
        {
            var result = CompanyRepository.AddCompany(Record);
            return result;      
        }
        [HttpPost]
        public string DeleteCompany(int[] CompanyId)
        {
            var result = CompanyRepository.DeleteCompany(CompanyId);
            return result;   
        }
        [HttpGet]
        public ActionResult GetCompany(int CompanyId)
        {
            var s = CompanyRepository.GetCompany(CompanyId);
            return PartialView("AddCompanyView", s);
        }
        public string UpdateCompany(CompanyInformation update)
        {
            var result = CompanyRepository.UpdateCompany(update);
            return result;
        }
        public ActionResult AuditRecord()
        {
            ListInformation list = new ListInformation();
            list.Companies=CompanyRepository.Companys();
            list.Groups = GroupRepository.AllGroups();
            list.CompanyGroups = CompanyGroupRepository.CompanyGroups();
            return PartialView(list);
        }
        public string AddAuditRecord(int[] Groupid,int Companyid,int Controlid)
        {
            var result = AuditRecordRepository.AddAuditRecord(Groupid,Companyid,Controlid);
            return result;
        }
        public ActionResult DeleteRecord()
        {
            ListInformation list = new ListInformation();
            list.Companies = CompanyRepository.Companys();
            list.Groups = GroupRepository.AllGroups();
            list.CompanyGroups = CompanyGroupRepository.CompanyGroups();
            return PartialView("DeleteRecordView",list);
        }
        public string DeleteRecordControl(int[] Groupid, int Companyid)
        {
            var result = CompanyGroupRepository.DeleteRecord(Groupid, Companyid);
            return result;
        }
    }
}