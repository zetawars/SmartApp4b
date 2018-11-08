using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartApp.Models.ViewModels;
using System.IO;
using SmartApp.Models.Repositories;

namespace SmartApp.Controllers
{
    public class DashboardActivityController : PanelController
    {
        FASRepository FASRepo { get; set; }

        public DashboardActivityController()
        {
            this.FASRepo = new FASRepository();
        }

        // GET: DashboardActivity
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActivityDetails()
        {
            ActivityDetails_VM displayVM = new ActivityDetails_VM();

            FASViewModel vm = new FASViewModel();
            vm.SelectedCompanies.Add(user.CompCode.ToString());
            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            try
            {
                displayVM.FASBox = FASRepo.GetCompanyCount("FAS", FASRepo.GetWhereClause(vm, "FAS", 1, user.UserID), user.UserID).First();
                displayVM.DemoBox = FASRepo.GetCompanyCount("Demo", FASRepo.GetWhereClause(vm, "Demo", 1, user.UserID), user.UserID).First();
                displayVM.FDBox = FASRepo.GetCompanyCount("F/D", FASRepo.GetWhereClause(vm, "F/D", 1, user.UserID), user.UserID).First();
                
            }
            catch (Exception ex)
            {
                //displayVM = new ActivityDetails_VM();
                
            }
            return View(displayVM);
        }

        public ActionResult CropDetails()
        {
            return View();
        }

        public ActionResult ProductsDetails()
        {
            var model = new MyViewModel()
            {
                Images = Directory.EnumerateFiles(Server.MapPath("~/Images/AGRI")).Select(fn => "~/Images/AGRI/" + Path.GetFileName(fn)),
            };
            return View(model);
        }

        public ActionResult UploadImages()
        {
            var model = new MyViewModel()
            {
                Images = Directory.EnumerateFiles(Server.MapPath("~/Images/AGRI"))
                                  .Select(fn => "~/Images/AGRI/" + Path.GetFileName(fn)),
              
               
        };
            return View(model);

        }

      

    }
}