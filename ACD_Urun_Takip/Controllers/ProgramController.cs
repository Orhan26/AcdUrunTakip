using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{
    public class ProgramController : Controller
    {
        // GET: Program
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<ProgramInfo> programList = ProgramRepository.AllPrograms();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in programList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input id='chk' class='thfirst-check' type='checkbox' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' value=" + item.ProgramId.ToString() + " />");
                colums.Add("ProgramName", item.ProgramName);
                columsList.Add(colums);
            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddProgramView()
        {
            ProgramInfo d = new ProgramInfo();
            d.ProgramName = null;
            return PartialView(d);
        }
        public string AddProgram(ProgramInfo newProgram)
        {
            var result = ProgramRepository.AddProgram(newProgram);
            return result;
        }
        [HttpPost]
        public string DeleteProgram(int[] programId)
        {
            var result = ProgramRepository.DeleteProgram(programId);
            return result;
        }
        public string UpdateProgram(ProgramInfo update)
        {
            var result = ProgramRepository.UpdateProgram(update);
            return result;
        }
        [HttpGet]
        public ActionResult GetProgram(int programId)
        {
            var program = ProgramRepository.GetProgram(programId);
            return PartialView("AddProgramView", program);
        }
    }
}