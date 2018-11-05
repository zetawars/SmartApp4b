using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{
    public class DashboardActivityController : Controller
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
    }
}