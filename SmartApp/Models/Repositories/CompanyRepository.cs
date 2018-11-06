using Dapper;
using SmartApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartApp.Models.Repositories
{
    public class CompanyRepository : DBRepository
    {
        public CompanyRepository()
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

        public Company GetCompany(int CompanyID)
        {
            string query = $@" SELECT distinct Compcode, compabb FROM Viewuserzone where Compcode= {CompanyID}; ";
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<Company>(query);
            }

        }
    }
}