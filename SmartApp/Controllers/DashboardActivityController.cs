using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartApp.Models.ViewModels;
using System.IO;


namespace SmartApp.Controllers
{
    public class DashboardActivityController : PanelController
    {
        
        // GET: DashboardActivity
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActivityDetails()
        {
            return View();
        }

        public ActionResult CropDetails()
        {
            return View();
        }

        public ActionResult ProductsDetails()
        {
            return View();
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