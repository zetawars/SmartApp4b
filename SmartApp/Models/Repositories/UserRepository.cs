using SmartApp.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;

namespace SmartApp.Models.Repositories
{
    public class UserRepository : DBRepository
    {
        public User Get(string UserLogin, string Password)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<User>("exec [dbo].[spDashboardLogin]  @UserLogin, @Password", new { UserLogin, Password});
            }
        }

        public dynamic GetRoles()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return  connection.Query("exec [dbo].[spDashboardRolesList]");
            }
        }

        public dynamic GetDashboardMenu()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query("select * from tblDashboardMenu");
            }
        }
        public IEnumerable<dynamic> GetDashboardMenuSelected(int VID=0)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query($" SELECT M.VID, M.VName, isnull( UG.MenuID,0) as MenuID , ISNULL(UG.VID, 0) AS UGID , case when ISNULL(UG.VID, 0) = 0 then 0 else 1 end as IsSelected  FROM(select * from dbo.tblDashboardUserGroup where GroupID ={VID} ) AS UG RIGHT OUTER JOIN dbo.tblDashboardMenu AS M ON UG.MenuID = M.VID ");
            }
        }
        public dynamic ClearMenuStatus()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Execute("UPDATE tblDashboardMenu SET isActive = 0");
            }
        }
        public dynamic InsertUpdateData(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
               return connection.Query("UPDATE tblDashboardMenu SET isActive = 1 WHERE VID = " +id);
            }

        }

        public dynamic GetUserGroupList(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query("select * from tblDashboardUserGroup  WHERE GroupID = " + id);
            }

        }

        public dynamic InsertTblUserGroup(int MenuId , int GroupId , int EntryUserId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
               
                DateTime Transdatetime = DateTime.Now;
                return connection.Query("exec spInsertDashboardUserGroup @MenuID,@GroupID,@EntryUserID", new { MenuId, GroupId, EntryUserId});

                //return connection.Query($"INSERT INTO tblDashboardUserGroup (MenuID, GroupID, EntryUserID, Transdatetime)VALUES('{MenuId}', '{GroupId}', '{EntryUserId}', '{Transdatetime}'); ");
            }

        }
        
    }
}