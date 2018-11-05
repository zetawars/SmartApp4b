using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{
    public class DemoController : PanelController
    {
        [HttpGet]
        public ActionResult DemoEntryForm()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult DemoEntryForm(DemoEntry Entry)
        //{
        //    return View();
        //}
    }
}