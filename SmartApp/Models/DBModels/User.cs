using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartApp.Models.DBModels
{
    public class User
    {
        public int Uid { get; set; }
        public int UserLogin { get; set; }
        public string Userfullname { get; set; }
        public string Userpassword { get; set; }
        public int CompCode { get; set; }
        public int RegionID { get; set; }
        public string ZoneID { get; set; }
        public string TerritoryID { get; set; }
        public string Compabb { get; set; }  
        public string Compname { get; set; }
        public string RegionName { get; set; }
        public string ZoneName { get; set; }
        public string TerritoryName { get; set; }
        public int FYear { get; set; }
        public string GroupCode { get; set; }
        public int IsHO { get; set; }

        public string Emp_level { get; set; }
        public DateTime ServerDateTime { get; set; }
    }
}


