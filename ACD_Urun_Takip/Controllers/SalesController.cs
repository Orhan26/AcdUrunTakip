using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Hardware;
using ACD_Urun_Takip.Repository.Modul;
using ACD_Urun_Takip.Repository.Program;
using ACD_Urun_Takip.Repository.Satis;

namespace ACD_Urun_Takip.Controllers
{
    public class SalesController : Controller
    {
        // GET: Satis
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<SalesInfo> salesList = SalesRepository.AllSales();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in salesList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' class='thfirst-check' type='checkbox' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' value=" + item.SalesId.ToString() + " />");
                colums.Add("CompanyName", item.Company.CompanyName);
                colums.Add("ProgramName", item.Program.ProgramName);               
                colums.Add("ModuleName", item.Module == null? "":item.Module.ModuleName);               
                colums.Add("HardwareName", item.Hardware==null?"":item.Hardware.HardwareName);
                colums.Add("HardwareQty", item.HardwareQty);
                colums.Add("UserId", item.UserId);
                colums.Add("Date", item.Date.ToString());
                columsList.Add(colums);
            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddSalesView()
        {
            ListInformation list = new ListInformation();
            list.Companies = CompanyRepository.Companys();
            list.ProgramInfos = ProgramRepository.AllPrograms();
            list.Hardwares = HardwareRepository.AllHardWares();
            list.SalesInfo = null;
            return PartialView(list);
        }
         [HttpPost]      
        public string AddSales(SalesInfo newSales)
        {
            var result = SalesRepository.AddSales(newSales);
            return result;
        }
        [HttpGet]
        public ActionResult GetSales(int salesId)
        {
            ListInformation listInfo = new ListInformation();
            listInfo.Companies = CompanyRepository.Companys();
            listInfo.ProgramInfos = ProgramRepository.AllPrograms();
            listInfo.Hardwares = HardwareRepository.AllHardWares();
            listInfo.SalesInfo = SalesRepository.GetSales(salesId);
            listInfo.Modules = ModulRepository.Modules().Where(o => o.Program.ProgramId == listInfo.SalesInfo.Program.ProgramId).ToList();

            return PartialView("AddSalesView", listInfo);
        }
        public string UpdateSales(SalesInfo update)
        {
            var result=SalesRepository.UpdateSales(update);
            return result;
        }
        public string DeleteSales(int[] salesId)
        {
            var result = SalesRepository.DeleteSales(salesId);
            return result;
        }
    }
}