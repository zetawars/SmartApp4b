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
            AttendanceViewModel vm = new AttendanceViewModel();
            vm.DateFrom = DateTime.Parse(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"));
            vm.DateTo = DateTime.Now;
            //var list = AttendanceRepo.GetDetails("","","",0);

            var list = AttendanceRepo.GetDetails("FirstCheckIn", AttendanceRepo.GetWhereClause(vm,1), "", user.UserID);
            ViewBag.ListDetails = list;
            int counter = 0;
            foreach (var item in list)
            {
                string temp = item.VType;
                if (temp.Equals("Absent"))
                {
                    counter++;
                }
            }


            int total = list.Count();
            int final = list.Count() - counter;
            TempData["PresentCount"] = final;
            TempData["TotalCount"] = total;
            int percentage = (int)Math.Round((double)(100 * final) / total);
            TempData["Percent"] = percentage;



            return View();
        }
        public ActionResult DashboardDetails()
        {
            return View();
        }
        
       
        public ActionResult DashboardLevel2()
        {
            AttendanceViewModel vm = new AttendanceViewModel();
            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            vm.SelectedCompanies.Add(user.CompCode.ToString());
          
            var k = AttendanceRepo.GetRegionBoxes("FirstCheckIn", AttendanceRepo.GetWhereClause(vm, 1), AttendanceRepo.GetWhereAbClause(vm, 1, user.UserID), user.UserID);
            return View(k);

        }
        public ActionResult DashboardLevel3(string Region)
        {
            AttendanceViewModel vm = new AttendanceViewModel();

            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            vm.SelectedCompanies.Add(user.CompCode.ToString());
            if (!string.IsNullOrEmpty(Region))
            {

                Region region = AttendanceRepo.GetRegion(int.Parse(Region));
                ViewBag.Region = region.Name;
                vm.SelectedRegions.Add(Region);
            }
            var k = AttendanceRepo.GetZoneBoxes("FirstCheckIn", AttendanceRepo.GetWhereClause(vm, 1), AttendanceRepo.GetWhereAbClause(vm, 1, user.UserID), user.UserID);


            return View(k);
        }
        public ActionResult DashboardLevel4(string Region, string Zone)
        {
            AttendanceViewModel vm = new AttendanceViewModel();

            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            vm.SelectedCompanies.Add(user.CompCode.ToString());
            if (!string.IsNullOrEmpty(Region))
            {

                Region region = AttendanceRepo.GetRegion(int.Parse(Region));
                ViewBag.Region = region.Name;
                vm.SelectedRegions.Add(Region);
            }
            if (!string.IsNullOrEmpty(Zone))
            {
                Zone zone = AttendanceRepo.GetZone(Zone);
                ViewBag.Zone = zone.Name;
                vm.SelectedZones.Add(Zone);

            }

            var k = AttendanceRepo.GetTerritoryBoxes("FirstCheckIn", AttendanceRepo.GetWhereClause(vm, 1), AttendanceRepo.GetWhereAbClause(vm, 1, user.UserID), user.UserID);


            return View(k);
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
        public ActionResult Attendance(string Region, string Zone, string Territory)
        {
            AttendanceViewModel vm = new AttendanceViewModel();
            vm.SelectedCompanies = new List<string>();
            vm.Companies = CompanyRepo.GetCompanies(user.UserID);
            vm.SelectedCompanies = new List<string>();
            vm.SelectedCompanies.Add(user.CompCode.ToString());

            vm.Regions = AttendanceRepo.GetRegions(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), user.UserID);


            vm.SelectedRegions = new List<string>();
            if (!string.IsNullOrEmpty(Region))
            {
                vm.SelectedRegions.Add(Region);
            }
            vm.Zones = AttendanceRepo.GetZones(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.Regions.Select(x => x.ID.ToString()).ToList(), user.UserID);
            vm.SelectedZones = new List<string>();
            if (!string.IsNullOrEmpty(Zone))
            {
                vm.SelectedZones.Add(Zone);
            }


            vm.Territories = AttendanceRepo.GeTerritories(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.Regions.Select(x => x.ID.ToString()).ToList(), vm.Zones.Select(x => x.ID.ToString()).ToList(), user.UserID);
            vm.SelectedTerritories = new List<string>();
            if (!string.IsNullOrEmpty(Territory))
            {
                vm.SelectedTerritories.Add(Territory);
            }

            vm.DateFrom = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            vm.DateTo = DateTime.Now;
            //var list = AttendanceRepo.GetDetails("","","",0);

            var list = AttendanceRepo.GetDetails("FirstCheckIn",AttendanceRepo.GetWhereClause(vm,1),AttendanceRepo.GetWhereAbClause(vm,1,user.UserID), user.UserID);
            ViewBag.ListDetails = list;
            
           
            //string mycount = list.Where(x => x.VType).Equals("Absent").ToString();
            
            //int total = list.Count();
            //int final= list.Count() - Convert.ToInt32(mycount);
            //TempData["PresentCount"] = final;
            //TempData["TotalCount"] = total;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Attendance(AttendanceViewModel vm)
        {

            if (vm.SelectedCompanies == null)
            {
                vm.SelectedCompanies = new List<string>();
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
            vm.Companies = CompanyRepo.GetCompanies(user.UserID);
            vm.Regions = AttendanceRepo.GetRegions(1, vm.SelectedCompanies.Select(x => x.ToString()).ToList(), user.UserID);
            vm.Zones = AttendanceRepo.GetZones(1, vm.SelectedCompanies.Select(x => x.ToString()).ToList(), vm.SelectedRegions.Select(x => x.ToString()).ToList(), user.UserID);
            vm.Territories = AttendanceRepo.GeTerritories(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.SelectedRegions.Select(x => x.ToString()).ToList(), vm.SelectedZones.Select(x => x.ToString()).ToList(), user.UserID);
            //ViewBag.CreditDebit = AttendanceRepo.GetList(AttendanceRepo.GetWhereClause(vm));

            ViewBag.WhereClause = AttendanceRepo.GetWhereClause(vm, 1);
            ViewBag.WhereClauseAb = AttendanceRepo.GetWhereAbClause(vm, 1, user.UserID);
            //var list = AttendanceRepo.GetDetails("FirstCheckIn", AttendanceRepo.GetWhereClause(vm), AttendanceRepo.GetWhereAbClause(vm), user.UserID);
            var list = AttendanceRepo.GetDetails("FirstCheckIn", AttendanceRepo.GetWhereClause(vm, 1), AttendanceRepo.GetWhereAbClause(vm, 1, user.UserID), user.UserID);

            ViewBag.ListDetails = list;
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

        //public ActionResult GetDetailsList()
        //{
        //    var list = AttendanceRepo.GetDetails("FirstCheckIn", AttendanceRepo.GetWhereClause(vm), "", user.UserID);
        //    ViewBag.ListDetails = list;


        //    string mycount = list.Where(x => x.VType).Equals("Absent").ToString();

        //    int total = list.Count();
        //    int final = list.Count() - Convert.ToInt32(mycount);
        //    TempData["PresentCount"] = final;
        //    TempData["TotalCount"] = total;
        //    return RedirectToAction("Dashboard");
        //}


    }
}