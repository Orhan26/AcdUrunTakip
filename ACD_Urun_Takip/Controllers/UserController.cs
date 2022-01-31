using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using ACD_Urun_Takip.Repository.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Controllers
{
    [Authorize]
    //[ErrorHandle(View = "Error500")]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            int userId = Convert.ToInt32(User.Identity.Name.Split('_')[1]);
            ProfileModel profile = UserRepository.GetProfile(userId);
            return View(profile);
        }
        public ActionResult GetAll()
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            DataSet ds = UserRepository.GetUsers(userId);
            return View(ds);
        }
        [HttpPost]
        public JsonResult CreateUser(CreateUserModel model)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = UserRepository.CreateUser(model,userId);
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUser(EditUserModel model)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = UserRepository.UpdateUser(model, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetUser(long userid)
        {
            EditUserModel model = UserRepository.GetUser(userid);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [CheckRole(AllowedRoles = new string[] { "Admin", "Acd" })]
        public ActionResult UserMenu()
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            DataTable table = UserRepository.GetUserMenu(userId);            
            return View(table);
        }
        [HttpPost]
        public ActionResult MenuGroups(long groupid)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            MenuProfileGroupModel menus = UserRepository.GetMenuGroupById(groupid,userId);
            return PartialView(menus);
        }

        [HttpPost]
        //[HasPermission(Models.Home.MenuPermission.Delete)]
        public JsonResult DeleteUser(long userid)
        {
            AjaxResult result = UserRepository.DeleteUser(userid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateUserGroup(string usgname)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = UserRepository.CreateUserGroup(usgname,userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateMenuGroup(SysMenu sysMenu)
        {
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('_')[1]);
            AjaxResult result = UserRepository.UpdateMenuGroup(sysMenu, userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}