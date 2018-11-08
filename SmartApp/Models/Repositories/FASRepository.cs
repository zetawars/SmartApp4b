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
    public class FASRepository : DBRepository
    {

        public FASRepository()
        {
        }


     
        public IEnumerable<dynamic> GetDetails(string VType, string CWhere,  int Uid)
        {
            
            //string query = $@"exec spsaFirstCheckIn"+" "+" "+" "+0;
            using (var connection = new SqlConnection(ConnectionString))
            {
               

                string query = $"exec [dbo].[spSaRptDetails] '{VType}','{CWhere.Replace("'","''")}',{Uid}";
                return connection.Query(query).ToList();
            }

        }

        public IEnumerable<dynamic> GetPresrent(string VType,  string CWhereAb, int Uid)
        {
            string Where = $" where 1 = 1";
            Where += $" and VTime1 !=0 AND VDate BETWEEN '{(DateTime.Now.AddMonths(-1)).ToString("yyyy-MM-dd")}' AND '{(DateTime.Now).ToString("yyyy-MM-dd")}'";
            string CWhere = Where;
            //string query = $@"exec spsaFirstCheckIn"+" "+" "+" "+0;
            using (var connection = new SqlConnection(ConnectionString))
            {


                string query = $"exec [dbo].[spsaFirstCheckIn] '{VType}','{CWhere.Replace("'", "''")}','{CWhereAb}',{Uid}";
                return connection.Query(query).ToList();
            }

        }



        public IEnumerable<Box> GetCompanyCount(string VType, string CWhere, int Uid)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string query = $"exec [dbo].[spSaFASGroup] 'Company', '{VType}','{CWhere.Replace("'", "''")}',{Uid}";
                return connection.Query<Box>(query).ToList();
            }
        }

        public IEnumerable<dynamic> GetRegionBoxes(string VType, string CWhere,  int Uid)
        {  
            using (var connection = new SqlConnection(ConnectionString))
            {
                string query = $"exec [dbo].[spSaFASGroup] 'Region', '{VType}','{CWhere.Replace("'", "''")}',{Uid}";
                return connection.Query(query).ToList();
            }
        }

        public IEnumerable<dynamic> GetZoneBoxes(string VType, string CWhere,  int Uid)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string query = $"exec [dbo].[spSaFASGroup] 'Zone', '{VType}','{CWhere.Replace("'", "''")}',{Uid}";
                return connection.Query(query).ToList();
            }
        }

        public IEnumerable<dynamic> GetTerritoryBoxes(string VType, string CWhere, int Uid)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string query = $"exec [dbo].[spSaFASGroup] 'Territory', '{VType}','{CWhere.Replace("'", "''")}',{Uid}";
                return connection.Query(query).ToList();
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


        public string GetWhereClause(FASViewModel vm, string Type, int SessionGroupType, int UserID)
        {
            if (vm.DateFrom == null)
            {
                vm.DateFrom = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-") + "01");
            }
            if (vm.DateTo == null)
            {
                vm.DateTo = DateTime.Now;
            }

            string Where = $"Where ( T.Compcode in ({string.Join(",", vm.SelectedCompanies)}) ) ";
            if (vm.SelectedRegions != null && vm.SelectedRegions.Where(x => !string.IsNullOrWhiteSpace(x)).ToList().Count > 0)
            {
                Where += $" AND T.Regionid in ({string.Join(",", vm.SelectedRegions)})";
            }
            if (vm.SelectedZones == null || vm.SelectedZones.Count == 0)
            {
                if (SessionGroupType == 1)
                {
                    Where += $" AND (T.Zoneid in ( SELECT distinct [Zoneid] FROM zone WHERE [Compcode] in ({string.Join(",", vm.SelectedCompanies)})) ) ";
                }
                else
                {
                    Where += $" AND (T.Zoneid in ( SELECT distinct [Zoneid] FROM Viewuserzoneregion WHERE [Compcode] in ({string.Join(",", vm.SelectedCompanies)}) AND Uid = {UserID}) ) ";
                }
            }
            else
            {
                Where += $"  AND  ( T.zoneid in  ({string.Join(",",vm.SelectedZones)}) )";
            }
            if (vm.SelectedTerritories != null && vm.SelectedTerritories.Count > 0)
            {
                Where += $"  AND  ( T.Territoryid in  ({string.Join(",", vm.SelectedTerritories)}) )";
            }
            if (vm.SelectedDistricts != null && vm.SelectedDistricts.Count > 0)
            {        
                Where += $"  AND isnull(D.VID,0) = {vm.SelectedDistricts.First()}  ";
            }
            if (vm.SelectedVillages != null && vm.SelectedVillages.Count > 0)
            {
                Where += $"  AND V.VID = {vm.SelectedVillages.First()}";
            }
            if (vm.SelectedFarmers != null && vm.SelectedFarmers.Count > 0)
            {
                Where += $"  AND F.VID = {string.Join(",", vm.SelectedFarmers.First())}";
            }
            if (vm.SelectedStatus != null && vm.SelectedStatus.Count > 0)
            {
                Where += $"  AND H.VStatus = {vm.SelectedStatus.First()}";
            }
            if (vm.SelectedCrops != null && vm.SelectedCrops.Count > 0)
            {
                Where += $" AND CR.CropID = {vm.SelectedCrops.First()}";
            }
            if (vm.SelectedCategories != null && vm.SelectedCategories.Count > 0)
            {
                Where += $"   AND  ( P.Categoryid in  ({string.Join(",", vm.SelectedCategories)}) )";
            }
            if (vm.SelectedProducts != null && vm.SelectedProducts.Count > 0)
            {
                Where += $"   AND  ( P.Productid in   ({string.Join(",", vm.SelectedProducts)}) )";
            }
            if (vm.SelectedSpecs != null && vm.SelectedSpecs.Count > 0)
            {
                Where += $"  AND  ( P.Specid in  ({string.Join(",", vm.SelectedTerritories)}) )";
            }
            if (Type == "FAS")
            {
                Where += $" AND (H.VisitDate between '{((DateTime)vm.DateFrom).ToString("yyyy-MM-dd")}' and '{((DateTime)vm.DateTo).ToString("yyyy-MM-dd")}' )";
            }
            else if (Type == "Demo")
            {
                Where += $" AND (H.TrialDate between '{((DateTime)vm.DateFrom).ToString("yyyy-MM-dd")}' and '{((DateTime)vm.DateTo).ToString("yyyy - MM - dd")}' )";
            }
            else if (Type == "F/D")
            {
                Where += $" AND (H.FDDate between '{((DateTime)vm.DateFrom).ToString("yyyy-MM-dd")}' and '{((DateTime)vm.DateTo).ToString("yyyy-MM-dd")}' )";
            }
            //Where += $" AND (H.VisitDate between '{((DateTime)vm.DateFrom).ToString("yyyy-MM-dd")}' and  '{((DateTime)vm.DateTo).ToString("yyyy-MM-dd")}' )";
            if (vm.AcreageTo != 0 && vm.AcrageFrom != 0)
            {
                Where += $" AND (H.TotAcreage between {vm.AcrageFrom} and {vm.AcreageTo} )";
            }
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


        public Region GetRegion(int RegionID)
        {
            string query = $@"
SELECT DISTINCT Regionid as ID, Name +'('+ cast([Compcode] as varchar(50))+ ')' as Name  
FROM dbo.Region  WHERE Regionid = {RegionID} order by Name ";
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<Region>(query);
            }
        }



        public Zone GetZone(string ZoneID)
        {
            string query = $@"
 SELECT distinct Zoneid as ID, Zonename +'('+ cast(Compcode as varchar(50))+ ')'  as Name 
FROM  zone WHERE Zoneid = {ZoneID} 
";      
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<Zone>(query);
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


        public Territory GetTerritory(string TerritoryID)
        {
            string query = string.Empty;
            query = $@" SELECT [Territoryid] as ID, [Name] +'('+ cast(Territoryheader.[Compcode] as varchar(50))+ ')' as Name FROM [Territoryheader] Where Territoryid = {TerritoryID} ";
           
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<Territory>(query);
            }
        }
    }
}