
using SmartApp.Filters;
using SmartApp.Models.Repositories;
using SmartApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{

    [RequireSession]
    public class PanelController : BaseController
    {
        public SessionUser user { get { return Session["User"] as SessionUser; } }
       
        public List<Company> SessionCompanies { get { return Session["Companies"] as List<Company>; } }

        public CompanyRepository CompanyRepo { get; set; }
        public PanelController()
        {
            this.CompanyRepo = new CompanyRepository();

        }


        public ActionResult UpdateSession(string RedirectURL, int CompanyID, DateTime DateFrom, DateTime DateTo)
        {
            Company comp = CompanyRepo.GetCompany(CompanyID);
            user.CompCode = CompanyID;
            user.CompanyName = comp.CompanyName;
            user.Compabb = comp.Compabb;
            user.DateTo = DateTo;
            user.DateFrom = DateFrom;

            return Redirect(RedirectURL);
        }
    }
}