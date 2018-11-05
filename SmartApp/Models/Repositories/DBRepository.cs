using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartApp.Models.Repositories
{
    public class DBRepository
    {
        public string ConnectionString { get; set; }
        public DBRepository()
        {
            this.ConnectionString = ConfigurationManager.ConnectionStrings["4BDATAConnectionString"].ConnectionString;
        }
    }
}