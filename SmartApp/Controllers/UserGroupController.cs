using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartApp.Models.Repositories;
using SmartApp.Models.DBModels;

namespace SmartApp.Controllers
{//
    public class UserGroupController : PanelController
    {
        public UserRepository UserRepo { get; set; }

        public UserGroupController()
        {
            this.UserRepo = new UserRepository();
        }
        // GET: UserGroup
        public ActionResult Index(int VID=0)
        {
            ViewBag.bagRoles = UserRepo.GetRoles();
            ViewBag.bagMenus = this.UserRepo.GetDashboardMenu();
            if (VID != 0)
            {
                ViewBag.bagMenusSelected = UserRepo.GetDashboardMenuSelected(VID);

            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(List<string> IsSelected, int GID)
        {
            
            UserRepo.ClearMenuStatus();
            if (IsSelected != null)
            {
                foreach (var item in IsSelected)
                {
                    int MenuID = Convert.ToInt32(item);
                    int EntryUserId = user.UserLogin;
                    UserRepo.InsertTblUserGroup(MenuID, GID, EntryUserId);
                    //InsertTblUserGroup(MenuID, GID);

                }
                var list = UserRepo.GetDashboardMenuSelected(GID);
                ViewBag.bagMenus = list;

            }
            else
            {
                var list = UserRepo.GetDashboardMenuSelected(GID);
                ViewBag.bagMenus = list;
            }

            return RedirectToAction("Index");
        }



        public JsonResult SelectedUserMenu(int GID = 0)
        {
            var list = UserRepo.GetDashboardMenuSelected(GID);

            return Json (list.Select(x=>x.MenuID),JsonRequestBehavior.AllowGet);
        }
        public void InsertTblUserGroup(int GID , int MenuID )
        {
                int EntryUserId = user.UserLogin;
                UserRepo.InsertTblUserGroup(GID, MenuID,  EntryUserId);
            
        }
      
    }
}