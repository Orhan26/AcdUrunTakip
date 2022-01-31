using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Modul;
using ACD_Urun_Takip.Repository.Program;

namespace ACD_Urun_Takip.Controllers
{
    public class ModuleController : Controller
    {
        // GET: Modul
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<ModuleInformation> modulList = ModulRepository.Modules();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in modulList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' class='thfirst-check' type='checkbox' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' value=" + item.ModuleId.ToString() + " />");
                colums.Add("ModuleName", item.ModuleName);
                colums.Add("ProgramName", item.Program.ProgramName);
                columsList.Add(colums);
            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddModuleView()
        {
            ListInformation list = new ListInformation();
            list.ProgramInfos = ProgramRepository.AllPrograms();
            list.Modules = ModulRepository.Modules();
            list.ModuleInformation = null;
            return PartialView("AddModule",list);
        }
        [HttpPost]
        public string AddModule(int programId,string moduleName)
        {
            var result = ModulRepository.AddModule(programId,moduleName);
            return result;
        }
        public string DeleteModule(int[] moduleId)
        {
            var result = ModulRepository.DeleteModul(moduleId);
            return result;
        }
        public ActionResult GetModule(int moduleId)
        {
            ListInformation list = new ListInformation();
            list.ProgramInfos = ProgramRepository.AllPrograms();
            list.Modules = ModulRepository.Modules();
            list.ModuleInformation = ModulRepository.GetModul(moduleId); 
            return PartialView("AddModule", list);
        }
        public string UpdateModule(int programId,string moduleName,int moduleId)
        {
            var result = ModulRepository.UpdateModul(programId, moduleName, moduleId);
            return result;
        }
    }
}