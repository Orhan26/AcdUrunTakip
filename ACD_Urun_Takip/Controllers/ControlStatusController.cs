using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.CompanyGroup;
using ACD_Urun_Takip.Repository.ControlStatus;
using ACD_Urun_Takip.Repository.Group;
using ACD_Urun_Takip.Repository.Questions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{ 
    public class ControlStatusController : Controller
    {
        // GET: ControlStatus
        public ActionResult Index()
        {
            //var result = ControlStatusRepository.AllControls();
            return View();
        }
        [HttpPost]
        public JsonResult JsonModel()
        {
            List<HrkCompanyAuditInfo> hardwareList = ControlStatusRepository.AllControls();
            List<Dictionary<string, string>> columsList = new List<Dictionary<string, string>>();
            foreach (var item in hardwareList)
            {
                Dictionary<string, string> colums = new Dictionary<string, string>();
                colums.Add("chk", "<input data-companyId="+item.SirketID.CompanyId+ " id='chk' onmouseout='IsInputOver(false)' onmouseover='IsInputOver(true)' name='chkbox' class='thfirst-check' type='checkbox' value=" + item.DenetimID+" />");
                colums.Add("CompanyName", item.SirketID.CompanyName);
                colums.Add("GroupName", item.GrupID.GroupName);
                colums.Add("Question", item.SoruID.Question);
                if(item.Cevap==true)
                {
                    colums.Add("reply", "<input style='margin-left:12px' type='checkbox' checked='checked' disabled='disabled' class='checkbox' />");
                }
                else
                {
                    colums.Add("reply", "<input style='margin-left:12px' type='checkbox' disabled='disabled' class='checkbox' />");
                }
                colums.Add("explanation",item.Aciklama);
                colums.Add("Date", item.Tarih.ToString());
               
                columsList.Add(colums);
            }
            Dictionary<string, object> mainData = new Dictionary<string, object>();
            mainData.Add("data", columsList);
            return Json(mainData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddControlStatus()
        {
            var companyGroupInfos = CompanyGroupRepository.CompanyGroupsFalse();
            return PartialView(companyGroupInfos);
        }
        public ActionResult AddQuestion()
        {
            ListInformation list = new ListInformation();
            list.CompanyGroups = CompanyGroupRepository.CompanyGroups();
            return PartialView("AddQuestion",list);
        }
        public string DeleteControl(int[] ControlStatusId)
        {
            var result = ControlStatusRepository.DeleteControl(ControlStatusId);
            return result;
        }
        public ActionResult GetControlStatus(int groupId)
        {
            var result = ControlStatusRepository.GetControlStatus(groupId);
            return PartialView("UpdateControlStatus",result);
        }
        public string AddAuditRecord(int[] groupId,int companyId, int controlId)
        {
            var result = ControlStatusRepository.AddAuditRecord(groupId,companyId,controlId);
            return result;
        }
        [HttpPost]
        public string UpdateControl(List<HrkCompanyAuditInfo> updateControlStatus)
        {
            var result = ControlStatusRepository.UpdateStatus(updateControlStatus);
            return result;
        }
        public string AllDelete(int companyId)
        {
            var result = ControlStatusRepository.AllDelete(companyId);
            return result;
        }
        public JsonResult GetQuestions(int groupId,int companyId)
        {
            List<QuestionInformation> questionsList = new List<QuestionInformation>();
            bool varmi;
            var question = QuestionRepository.GetQuestion(groupId);
            var record = ControlStatusRepository.RecordControl(companyId,groupId);
            for (int i = 0; i < question.Count; i++)
            {
                varmi = false;
                for (int j = 0; j < record.Count; j++)
                {
                    if (question[i].QuestionID==record[j].SoruID.QuestionID) {
                        varmi = true;
                    }
                }
                if(varmi==false)
                {
                    QuestionInformation questions = new QuestionInformation();
                    questions.QuestionID = question[i].QuestionID;
                    questions.Question = question[i].Question;
                    questionsList.Add(questions);
                }
            }
            return Json(questionsList, JsonRequestBehavior.AllowGet);
        }
        public string RecordQuestion(int groupId,int[] questionId,int companyId)
        {
            var result = ControlStatusRepository.AddQuestion(groupId, companyId, questionId);
            return result;
        }
    }
}