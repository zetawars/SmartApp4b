using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace System
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MessageType { get; set; }
        public string NotificationType { get; set; }
        public string Icon { get; set; }
        public bool IsAjaxMessage { get; set; }
        public bool IsViewMessage { get; set; }
        public bool IsRedirectMessage { get; set; }
        public string Code { get; set; }
        public string URL { get; set; }
        public bool Viewed { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserId { get; set; }
        public string UserType { get; set; }

        public NotificationMessage()
        {
            IsAjaxMessage = true;
            IsViewMessage = true;
            IsRedirectMessage = true;
        }
    }
}