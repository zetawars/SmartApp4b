using SmartApp.Models.Repositories;
using SmartApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{
    public class FASController : PanelController
    {
        public CreditDebitRepository CRepo { get; set; }
        public FASRepository FASRepo { get; set; }

        public FASController()
        {
            CRepo = new CreditDebitRepository();
            FASRepo = new FASRepository();

        }

        public ActionResult DashboardLevel2(string Type)
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = "FAS";
            }
            FASViewModel vm = new FASViewModel();
            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            vm.SelectedCompanies.Add(user.CompCode.ToString());

            var k = FASRepo.GetRegionBoxes(Type, FASRepo.GetWhereClause(vm,Type, 1, user.UserID), user.UserID);
            return View(k);

        }
        public ActionResult DashboardLevel3(string Type, string Region)
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = "FAS";
            }
            FASViewModel vm = new FASViewModel();
            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            vm.SelectedCompanies.Add(user.CompCode.ToString());
            if (!string.IsNullOrEmpty(Region))
            {

                Region region = FASRepo.GetRegion(int.Parse(Region));
                ViewBag.Region = region.Name;
                vm.SelectedRegions.Add(Region);
            }
            var k = FASRepo.GetZoneBoxes(Type, FASRepo.GetWhereClause(vm,Type, 1, user.UserID), user.UserID);


            return View(k);
        }
        public ActionResult DashboardLevel4(string Type, string Region, string Zone)
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = "FAS";
            }
            FASViewModel vm = new FASViewModel();

            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            vm.SelectedCompanies.Add(user.CompCode.ToString());
            if (!string.IsNullOrEmpty(Region))
            {

                Region region = FASRepo.GetRegion(int.Parse(Region));
                ViewBag.Region = region.Name;
                vm.SelectedRegions.Add(Region);
            }
            if (!string.IsNullOrEmpty(Zone))
            {
                Zone zone = FASRepo.GetZone(Zone);
                ViewBag.Zone = zone.Name;
                vm.SelectedZones.Add(Zone);

            }

            var k = FASRepo.GetTerritoryBoxes(Type, FASRepo.GetWhereClause(vm,Type, 1, user.UserID), user.UserID);


            return View(k);
        }




        [HttpGet]
        public ActionResult Details(string Type, string Region, string Zone, string Territory)
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = "FAS";
            }
            FASViewModel vm = new FASViewModel();
            vm.SelectedCompanies = new List<string>();
            vm.Companies = CompanyRepo.GetCompanies(user.UserID);
            vm.SelectedCompanies = new List<string>();
            vm.SelectedCompanies.Add(user.CompCode.ToString());

            vm.Regions = FASRepo.GetRegions(1, vm.SelectedCompanies, user.UserID);


            vm.SelectedRegions = new List<string>();
            if (!string.IsNullOrEmpty(Region))
            {
                Region region = FASRepo.GetRegion(int.Parse(Region));
                ViewBag.Region = region.Name;

                vm.SelectedRegions.Add(Region);
            }

            if (vm.SelectedRegions.Count != 0)
            {
                vm.Zones = FASRepo.GetZones(1, vm.SelectedCompanies, vm.SelectedRegions, user.UserID);

            }
            else
            {
                vm.Zones = FASRepo.GetZones(1, vm.SelectedCompanies, vm.Regions.Select(x => x.ID.ToString()).ToList(), user.UserID);
            }

            vm.SelectedZones = new List<string>();
            if (!string.IsNullOrEmpty(Zone))
            {
                Zone zone = FASRepo.GetZone(Zone);
                ViewBag.Zone = zone.Name;
                vm.SelectedZones.Add(Zone);
            }

            if (vm.SelectedZones.Count != 0)
            {
                if (vm.SelectedRegions.Count != 0)
                {
                    vm.Territories = FASRepo.GeTerritories(1, vm.SelectedCompanies, vm.SelectedRegions, vm.SelectedZones, user.UserID);
                }
                else
                {
                    vm.Territories = FASRepo.GeTerritories(1, vm.SelectedCompanies, vm.Regions.Select(x => x.ID.ToString()).ToList(), vm.SelectedZones, user.UserID);
                }
            }
            else
            {
                vm.Territories = FASRepo.GeTerritories(1, vm.SelectedCompanies, vm.Regions.Select(x => x.ID.ToString()).ToList(), vm.Zones.Select(x => x.ID.ToString()).ToList(), user.UserID);
            }

            vm.SelectedTerritories = new List<string>();
            if (!string.IsNullOrEmpty(Territory))
            {
                Territory territory = FASRepo.GetTerritory(Territory);
                ViewBag.Territory = territory.Name;

                vm.SelectedTerritories.Add(Territory);
            }

            vm.DateFrom = user.DateFrom;
            vm.DateTo = user.DateTo;
            var list = FASRepo.GetDetails(Type, FASRepo.GetWhereClause(vm,Type, 1,user.UserID),  user.UserID);
            ViewBag.ListDetails = list;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Details(FASViewModel vm)
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
            vm.Regions = FASRepo.GetRegions(1, vm.SelectedCompanies.Select(x => x.ToString()).ToList(), user.UserID);
            vm.Zones = FASRepo.GetZones(1, vm.SelectedCompanies.Select(x => x.ToString()).ToList(), vm.SelectedRegions.Select(x => x.ToString()).ToList(), user.UserID);
            vm.Territories = FASRepo.GeTerritories(1, vm.Companies.Select(x => x.Compcode.ToString()).ToList(), vm.SelectedRegions.Select(x => x.ToString()).ToList(), vm.SelectedZones.Select(x => x.ToString()).ToList(), user.UserID);
         
            ViewBag.WhereClause = FASRepo.GetWhereClause(vm,"FAS", 1, user.UserID);

            var list = FASRepo.GetDetails("FAS", FASRepo.GetWhereClause(vm,"FAS", 1, user.UserID),  user.UserID);

            ViewBag.ListDetails = list;
            return View(vm);
        }
    }
}