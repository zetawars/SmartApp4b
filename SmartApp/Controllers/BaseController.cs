using Microsoft.AspNet.SignalR;
using SmartApp.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{
    //[ErrorHandler]
    public class BaseController : Controller
    {
        protected IHubContext Context { get; set; }

        public BaseController()
        {
            this.Context = GlobalHost.ConnectionManager.GetHubContext<Notification>();
        }



        public virtual void Notify(NotificationMessage message)
        {
            if (message.IsAjaxMessage)
            {
                Context.Clients.Group(Request.UserHostAddress).Notify(message);
            }
            if (message.IsRedirectMessage)
            {
                TempData["NotificationMessage"] = message;
            }
            if (message.IsViewMessage)
            {
                ViewBag.NotificationMessage = message;
            }
        }

        public new RedirectToRouteResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
    }
}