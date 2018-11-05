using Dapper;
using SmartApp.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartApp.Models.Repositories
{
    public class CreditDebitRepository : DBRepository 
    {
        public List<CreditDebit> GetList()
        {
            string query = $@"
SELECT TOP (100) PERCENT Compabb, Regionname, ZoneName, TerritoryName, PartyName + ' (' + PartyCode + ')' AS PartyName, SUM(Debit) AS Debit, SUM(Credit) AS Credit
FROM     dbo.rptLedger AS H
--WHERE  (Compcode = 14) AND TerritoryName ='DG KHAN'
GROUP BY Compabb, Regionname, ZoneName, TerritoryName, PartyName, PartyCode
ORDER BY Compabb, Regionname, ZoneName, TerritoryName, PartyName
";
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<CreditDebit>(query).ToList();
            }
        }

    }
}