using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartApp.Models.ViewModels
{
    public class AttendanceViewModel
    {
        public List<Company> Companies { get; set; }
        public List<string> SelectedCompanies { get; set; }
        public List<Region> Regions { get; set; }
        public List<string> SelectedRegions { get; set; }
        public List<Zone> Zones { get; set; }
        public List<string> SelectedZones { get; set; }
        public List<Territory> Territories { get; set; }
        public List<string> SelectedTerritories { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public string SearchBox { get; set; }

        public AttendanceViewModel()
        {
            this.Companies = new List<Company>();
            this.SelectedCompanies = new List<string>();
            this.Regions = new List<Region>();
            this.SelectedRegions = new List<string>();
            this.Zones = new List<Zone>();
            this.SelectedZones = new List<string>();
            this.Territories = new List<Territory>();
            this.SelectedTerritories = new List<string>();
            this.DateTo = new DateTime();
            this.DateFrom = new DateTime();
            this.SearchBox = string.Empty;
        }
    }


    public class Company
    {
        public int Compcode { get; set; }
        public string Compabb { get; set; }
    }

    public class Region
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Zone
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class Territory
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


}