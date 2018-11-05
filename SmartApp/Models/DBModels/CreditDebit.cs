using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartApp.Models.DBModels
{
    public class CreditDebit
    {
        public string Compabb { get; set; }
        public string Regionname { get; set; }
        public string ZoneName { get; set; }
        public string TerritoryName { get; set; }
        public string PartyName { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
    }
}