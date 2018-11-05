using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SmartApp.Hubs
{
    public class Notification : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.SendMyMessage(name, message);
        }

        public void Notify(NotificationMessage message)
        {
            string username = Context.QueryString["username"];
            Clients.Group(username).Notify(message);
        }

        public override Task OnConnected()
        {
            string username = Context.QueryString["username"];
            Groups.Add(Context.ConnectionId, username);
            return base.OnConnected();
        }


        public void GetNotifications()
        {
            Clients.All.GetNotifications();
        }
    }

    public class MessagesRepository
    {
        readonly static string _connString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        public int BellNotificationListner(int UserID, string UserType)
        {
            //var messages = new List<NotificationMessage>();
            //int count = 0;
            //using (var connection = new SqlConnection(_connString))
            //{
            //    //Do not use COUNT(*) or Select * or anything that uses * in SqlDependency
            //    connection.Open();
            //    using (var command = new SqlCommand($"SELECT N.ID as TotalNotifications FROM [General].[NotificationMessage] N Where N.UserID = {UserID} AND N.UserType = '{UserType}' AND Viewed = 0", connection))
            //    {
            //        command.Notification = null;
            //        var dependency = new SqlDependency(command);
            //        dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
            //        if (connection.State == ConnectionState.Closed)
            //            connection.Open();
            //        SqlDataReader reader = command.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            count++;
            //        }
            //    }
            //}
            return 0;// count;
        }

        //public static List<NotificationMessage> GetBellNotificationMessages(int UserId, string UserType)
        //{
        //    List<NotificationMessage> result = new List<NotificationMessage>();
        //    DBHelper db = new DBHelper();
        //    string query = string.Empty;
        //    if (UserType == "User")
        //    {
        //        query = $"SELECT* FROM[General].[NotificationMessage] where UserID = {UserId} AND UserType = '{UserType}' AND Viewed = 0";
        //    }
        //    else
        //    {
        //        query = $"SELECT* FROM[General].[NotificationMessage] where UserID = {UserId} AND UserType = '{UserType}' AND Viewed = 0";
        //    }
        //    return DBHelper.GetList<NotificationMessage>();
        //}


        //public static List<NotificationMessage> GetBellUnReadNotificationMessages(int UserId, string UserType)
        //{
        //    List<NotificationMessage> result = new List<NotificationMessage>();
        //    DBHelper db = new DBHelper();
        //    return DBHelper.GetList<NotificationMessage>($"SELECT * FROM [General].[NotificationMessage] where UserID = {UserId} AND UserType = '{UserType}' AND Viewed = 0 Order by TimeStamp DESC");
        //}

        //public static List<NotificationMessage> GetList(int UserId, string UserType)
        //{
        //    List<NotificationMessage> result = new List<NotificationMessage>();
        //    DBHelper db = new DBHelper();
        //    return DBHelper.GetList<NotificationMessage>($"SELECT * FROM [General].[NotificationMessage] where UserID = {UserId} AND UserType = '{UserType}' Order by TimeStamp DESC");
        //}

        //internal static void MarkRead(string messageID)
        //{
        //    DBHelper db = new DBHelper();
        //    db.values.Add("@MessageID", messageID);
        //    DBHelper.ExecuteQuery("UPDATE [General].[NotificationMessage] SET Viewed = 1 Where ID = @MessageID", db.values);
        //}

        //internal static void MarkAllRead(int UserId, string UserType)
        //{
        //    DBHelper db = new DBHelper();
        //    DBHelper.ExecuteQuery($"UPDATE [General].[NotificationMessage] SET Viewed = 1 Where UserID = '{UserId}' AND UserType = '{UserType}'");
        //}


       
        //public static void Insert(NotificationMessage message)
        //{
        //    DBHelper db = new DBHelper();
        //    DBHelper.Insert(message, "General", "NotificationMessage");
        //}



        public virtual void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Notification>();
                context.Clients.All.GetNotifications();
            }
        }

       
    }

}