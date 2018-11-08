using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartApp.Models.ViewModels
{
    public class FASViewModel
    {
        public List<Company> Companies { get; set; }
        public List<string> SelectedCompanies { get; set; }
        public List<Region> Regions { get; set; }
        public List<string> SelectedRegions { get; set; }
        public List<Zone> Zones { get; set; }
        public List<string> SelectedZones { get; set; }
        public List<Territory> Territories { get; set; }
        public List<string> SelectedTerritories { get; set; }

        public List<District> Districts { get; set; }
        public List<string> SelectedDistricts { get; set; }


        public List<Village> Villages { get; set; } 
        public List<string> SelectedVillages { get; set; }

        public List<Farmer> Farmers { get; set; }
        public List<string> SelectedFarmers { get; set; }

        public List<Status> Status { get; set; }
        public List<string> SelectedStatus { get; set; }

        public List<Crop> Crops { get; set; }
        public List<string> SelectedCrops { get; set; }


        

        public List<Category> Categories { get; set; }
        public List<string> SelectedCategories { get; set; }


        public List<Product> Products { get; set; }
        public List<string> SelectedProducts { get; set; }

        public List<Spec> Specs { get; set; }
        public List<string> SelectedSpecs { get; set; }


        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public string SearchBox { get; set; }
        public int AcrageFrom { get; set; }
        public int AcreageTo { get; set; }
        public FASViewModel()
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

    public class District
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


    public class Village
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


    public class Farmer
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Status
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Crop
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Spec
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


}