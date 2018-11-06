using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartApp.Models.ViewModels
{
    public class MyViewModel
    {
        public IEnumerable<string> Images { get; set; }
        public string ImageName { get; set; }
        public IEnumerable<string> getPath { get; set; }

    }
    public class Common
    {
      
    }
}