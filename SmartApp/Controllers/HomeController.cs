using SmartApp.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartApp.Models.ViewModels;

namespace SmartApp.Controllers
{
    public class HomeController : PanelController
    {
        public CreditDebitRepository CRepo { get; set; }

        public AttendanceRepository AttendanceRepo { get; set; }
        public HomeController()
        {
            CRepo = new CreditDebitRepository();
            AttendanceRepo = new AttendanceRepository();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult DashboardDetails()
        {
            return View();
        }
        public ActionResult DashboardLevel2()
        {
            return View();
        }
        public ActionResult DashboardLevel3()
        {
            return View();
        }
        public ActionResult DashboardLevel4()
        {
            return View();
        }
        public ActionResult DashboardLevel5()
        {
            return View();
        }
        public ActionResult DrillDown()
        {
            return View(CRepo.GetList());

        }

        public ActionResult SelectedDetails()
        {
            return View();
        }
        public ActionResult Scoreboard()
        {
            return View();
        }



        public ActionResult TerritoryStats()
        {
            return View();
        }
        public ActionResult BusinessReview()
        {
            return View();
        }
        public ActionResult ActivityDetail()
        {
            return View();
        }
        public ActionResult SummaryReports()
        {
            return View();
        }



        [HttpGet]
        public ActionResult Attendance()
        {
            AttendanceViewModel vm = new AttendanceViewModel();
            vm.SelectedCompanies = new List<int>();
            vm.Companies = AttendanceRepo.GetCompanies(user.UserID);
            vm.Regions = AttendanceRepo.GetRegions(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), user.UserID);
            vm.SelectedRegions = new List<string>();
            vm.Zones = AttendanceRepo.GetZones(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.Regions.Select(x => x.ID.ToString()).ToList(), user.UserID);
            vm.Territories = AttendanceRepo.GeTerritories(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.Regions.Select(x => x.ID.ToString()).ToList(), vm.Zones.Select(x => x.ID.ToString()).ToList(), user.UserID);
            vm.DateFrom = DateTime.Parse(DateTime.Now.ToString("yyyy-MM") + "-01");
            vm.DateTo = DateTime.Now;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Attendance(AttendanceViewModel vm)
        {

            if (vm.SelectedCompanies == null)
            {
                vm.SelectedCompanies = new List<int>();
            }
            if (vm.SelectedRegions == null)
            {
                vm.SelectedRegions = new List<string>();
            }
            if (vm.SelectedTerritories == null)
            {
                vm.SelectedTerritories = new List<string>();
            }
            if (vm.SelectedZones == null)
            {
                vm.SelectedZones = new List<string>();
            }
            vm.Companies = AttendanceRepo.GetCompanies(user.UserID);
            vm.Regions = AttendanceRepo.GetRegions(1, vm.SelectedCompanies.Select(x => x.ToString()).ToList(), user.UserID);
            vm.Zones = AttendanceRepo.GetZones(1, vm.SelectedCompanies.Select(x => x.ToString()).ToList(), vm.SelectedRegions.Select(x => x.ToString()).ToList(), user.UserID);
            vm.Territories = AttendanceRepo.GeTerritories(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.SelectedRegions.Select(x => x.ToString()).ToList(), vm.SelectedZones.Select(x => x.ToString()).ToList(), user.UserID);
            ViewBag.CreditDebit = AttendanceRepo.GetList(AttendanceRepo.GetWhereClause(vm));

            ViewBag.WhereClause = AttendanceRepo.GetWhereClause(vm);
            return View(vm);
        }


        [HttpPost]
        public JsonResult CompanyChange(List<string> Companies)
        {

            List<Region> Regions = AttendanceRepo.GetRegions(1, Companies, user.UserID);
            List<Zone> Zones = AttendanceRepo.GetZones(1, Companies, Regions.Select(x => x.ID.ToString()).ToList(), user.UserID);
            List<Territory> Territories = AttendanceRepo.GeTerritories(1, Companies, Regions.Select(x => x.ID.ToString()).ToList(), Zones.Select(x => x.ID.ToString()).ToList(), user.UserID);
            var RegionDropdown = Regions.Select(x => new { id = x.ID, text = x.Name }).ToList();
            var ZoneDropdown = Regions.Select(x => new { id = x.ID, text = x.Name }).ToList();
            var TerritoryDropdown = Regions.Select(x => new { id = x.ID, text = x.Name }).ToList();

            return Json(new
            {
                RegionDropdown,
                ZoneDropdown,
                TerritoryDropdown
            });
        }


        [HttpPost]
        public JsonResult RegionChange(List<string> Companies, List<string> Regions)
        {
            List<Zone> Zones = AttendanceRepo.GetZones(1, Companies, Regions, user.UserID);
            List<Territory> Territories = AttendanceRepo.GeTerritories(1, Companies, Regions, Zones.Select(x => x.ID.ToString()).ToList(), user.UserID);
            var ZoneDropdown = Zones.Select(x => new { id = x.ID, text = x.Name }).ToList();
            var TerritoryDropdown = Territories.Select(x => new { id = x.ID, text = x.Name }).ToList();


            return Json(new
            {
                ZoneDropdown,
                TerritoryDropdown
            });
        }

        [HttpPost]
        public JsonResult ZoneChange(List<string> Companies, List<string> Regions, List<string> Zones)
        {
            List<Territory> Territories = AttendanceRepo.GeTerritories(1, Companies, Regions, Zones, user.UserID);
            var TerritoryDropdown = Territories.Select(x => new { id = x.ID, text = x.Name }).ToList();

            return Json(new { TerritoryDropdown });
        }
    }
}