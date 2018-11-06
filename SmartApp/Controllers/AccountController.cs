using SmartApp.Models.Repositories;
using SmartApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{
    public class AccountController : BaseController
    {
        public UserRepository UserRepo { get; set; }
        public CompanyRepository CompanyRepo { get; set; }

        public AccountController()
        {
            this.UserRepo = new UserRepository();
            this.CompanyRepo = new CompanyRepository();
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserCode, string Password, string FYear)
        {
            SessionUser user = new SessionUser(UserRepo.Get(UserCode, Password));
            
            if (user != null && user.UserID != 0)
            {
                List<Company> UserCompanies = CompanyRepo.GetCompanies(user.UserID);

                user.FYear = int.Parse(FYear);
                Session["Companies"] = UserCompanies;
                Session["User"] = user;

                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid Usercode or password";
            }
            return View();

        }
    }
}