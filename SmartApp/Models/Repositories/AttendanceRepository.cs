using Dapper;
using SmartApp.Models.DBModels;
using SmartApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartApp.Models.Repositories
{
    public class AttendanceRepository : DBRepository
    {

        public AttendanceRepository()
        {
        }


        public List<Company> GetCompanies(int UserID)
        {
            string query = $@" SELECT distinct Compcode, compabb FROM Viewuserzone where Uid= {UserID}; ";
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<Company>(query).ToList();
            }

        }



        public List<CreditDebit> GetList(string cWhere)
        {
            string query = $@"exec [SpSaDashboardSummery]";
            if (!string.IsNullOrWhiteSpace(cWhere))
            {
                query += $"'{cWhere.Replace("'", "''")}'";
            }
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<CreditDebit>(query).ToList();
            }
        }


        public string GetWhereClause(AttendanceViewModel vm)
        {
            string Where = $" where 1 = 1 ";
            if (vm.DateFrom == null)
            {
                vm.DateFrom = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-") + "01");
            }
            if (vm.DateTo == null)
            {
                vm.DateTo = DateTime.Now;
            }
            if (vm.SelectedCompanies!= null && vm.SelectedCompanies.Where(x=>!string.IsNullOrEmpty(x.ToString())).ToList().Count > 0)
            {
                Where += $" AND compcode IN ({string.Join(",",vm.SelectedCompanies)})";
            }
            if (vm.SelectedRegions != null && vm.SelectedRegions.Where(x=>!string.IsNullOrWhiteSpace(x)).ToList().Count > 0)
            {
                Where += $" AND Regionid IN ({string.Join(",", vm.SelectedRegions)})";
            }

            if (vm.SelectedZones != null && vm.SelectedZones.Count > 0)
            {
                Where += $" AND Zoneid IN ({string.Join(",", vm.SelectedZones)})";
            }
            if (vm.SelectedTerritories != null && vm.SelectedTerritories.Count > 0)
            {
                Where += $" AND Territoryid IN ({string.Join(",", vm.SelectedTerritories)})";
            }
            if (!string.IsNullOrWhiteSpace(vm.SearchBox))
            {
                Where += $"AND  (PartyName Like '%{vm.SearchBox}%' OR PartyCode Like '%{vm.SearchBox}%'  )";
            }
            Where += $" AND VDate BETWEEN '{((DateTime)vm.DateFrom).ToString("yyyy-MM-dd")}' AND '{((DateTime)vm.DateTo).ToString("yyyy-MM-dd")}'";
            return Where;
        }

        public List<Region> GetRegions(int SessionGroupType, List<string> Companies, int UserID)
        {
            string query = string.Empty;

            if (SessionGroupType == 1)
            {
                query = $@"
SELECT DISTINCT Regionid as ID, Name +'('+ cast([Compcode] as varchar(50))+ ')' as Name  
FROM dbo.Region  WHERE ([Compcode] in ({ string.Join(",", Companies) }))  order by Name ";

            }
            else
            {

                query = $@" SELECT DISTINCT dbo.Zone.Regionid as ID, dbo.Region.Name +'('+ cast(dbo.Userzone.[Compcode] as varchar(50))+ ')'  AS Name 
             FROM dbo.Userzone INNER JOIN dbo.Zone ON dbo.Userzone.Zoneid = dbo.Zone.Zoneid 
             INNER JOIN dbo.Region ON dbo.Zone.Regionid = dbo.Region.Regionid 
             WHERE (dbo.Userzone.[Compcode] in ({string.Join(",", Companies)})) AND (dbo.Userzone.Uid =  { UserID } ) order by Name ";

            }
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<Region>(query).ToList();
            }



        }


        public List<Zone> GetZones(int GroupType, List<string> Companies, List<string> Regions, int UserID)
        {
            string query = string.Empty;
            if (GroupType == 1)
            {
                query = $@"
 SELECT distinct Zoneid as ID, Zonename +'('+ cast(Compcode as varchar(50))+ ')'  as Name 
FROM  zone WHERE ([Compcode] in ({string.Join(", ", Companies)})) 
";
            }
            else
            {
                query = $@"
           SELECT distinct Zoneid as ID, Zonename + '(' + cast(Compcode as varchar(50)) + ')' as Name 
           FROM  viewuserzone WHERE ([Compcode] in ({string.Join(",", Companies)}))  AND (Uid = {UserID}) ";
            }

            if (Regions != null && Regions.Count > 0)
            {
                query += $" and  Regionid in ( {string.Join(",", Regions)}) ";
            }
            query += " order by Name";
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<Zone>(query).ToList();
            }

        }

        
        public List<Territory> GeTerritories(int GroupType, List<string>Companies, List<string> Regions, List<string> Zones ,int UserID)
        {
            string query = string.Empty;
            query = $@" SELECT [Territoryid] as ID, [Name] +'('+ cast(Territoryheader.[Compcode] as varchar(50))+ ')' as Name FROM [Territoryheader] INNER JOIN 
     Zone ON Territoryheader.Zoneid = Zone.Zoneid 
     WHERE (Territoryheader.[Compcode] in ({string.Join(",", Companies) })) ";
            if (Regions != null && Regions.Count > 0)
            {

                if (Zones != null && Zones.Count > 0)
                {
                    query += $" and Territoryheader.Zoneid in ({string.Join(",", Zones)}) and dbo.Zone.Regionid in ({string.Join(",", Regions)}) ";

                }
                else
                {
                    query += $@" and (dbo.Zone.Regionid in ({string.Join(",", Regions)}) and(Zone.Zoneid in (SELECT distinct[Zoneid] FROM Viewuserzoneregion WHERE[Compcode] in ({string.Join(",", Companies)}) AND Uid = {UserID}) ) ) ";
                }

            }
            else
            {
                if (Zones != null && Zones.Count > 0)
                {
                    query += $" and Territoryheader.Zoneid in ( {string.Join(",", Zones)} ) ";
                }
                else
                {
                    query += $" and (Zone.Zoneid in ( SELECT distinct  [Zoneid] FROM Viewuserzoneregion WHERE [Compcode] in ({string.Join(",", Companies)}) AND Uid = {UserID}) ) ";
                }
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<Territory>(query).ToList();
            }
        }
    }
}