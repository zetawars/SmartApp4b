using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartApp.Models.ViewModels
{
    public class ActivityDetails_VM
    {
        public Box FASBox { get; set; }
        public Box DemoBox { get; set; }
        public Box FDBox { get; set; }
        public ActivityDetails_VM()
        {
            this.FASBox = new Box();
            this.DemoBox = new Box();
            this.FDBox = new Box();
        }

    }

    public class Box
    {
        public string Per { get; set; }
        public string PP { get; set; }
        public string Total { get; set; }
        public Box()
        {
            this.Per = "0";
            this.PP = "0";
            this.Total = "0";
        }
    }
}