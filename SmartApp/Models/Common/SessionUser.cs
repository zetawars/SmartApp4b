using SmartApp.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public class SessionUser
    {

        public int UserID { get; set; }
        public int UserLogin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int CompCode { get; set; }
        public int RegionID { get; set; }
        public string ZoneID { get; set; }
        public string TerritoryID { get; set; }
        public string CompanyAbbrivation { get; set; }
        public string CompanyName { get; set; }
        public string RegionName { get; set; }
        public string ZoneName { get; set; }
        public string TerritoryName { get; set; }
        public int FYear { get; set; }
        public string GroupCode { get; set; }
        public int IsHO { get; set; }
        public string EmpLevel { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime ServerDateTime { get; set; }


        public SessionUser(User user)
        {
            if (user != null)
            {
                this.UserID = user.Uid;
                this.UserLogin = user.UserLogin;
                this.UserName = user.Userfullname;
                this.Password = user.Userpassword;
                this.CompCode = user.CompCode;
                this.RegionID = user.RegionID;
                this.ZoneID = user.ZoneID;
                this.TerritoryID = user.TerritoryID;
                this.CompanyAbbrivation = user.Compabb;
                this.CompanyName = user.Compname;
                this.RegionName = user.RegionName;
                this.ZoneName = user.ZoneName;
                this.TerritoryName = user.TerritoryName;
                this.FYear = user.FYear;
                this.GroupCode = user.GroupCode;
                this.IsHO = user.IsHO;
                this.EmpLevel = user.Emp_level;
                this.ServerDateTime = user.ServerDateTime;
                this.DateFrom = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-") + "01");
                this.DateTo = DateTime.Now;
            }
        }

        public SessionUser()
        {

        }
    }
}