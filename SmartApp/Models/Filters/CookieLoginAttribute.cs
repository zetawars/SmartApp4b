
using SmartApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Filters
{
    public class CookieLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (BaseController)filterContext.Controller;
            SessionUser _user = controller.Session["User"] as SessionUser;
            if (_user == null)
            {
                if (controller.Request.Cookies["LoginData"] != null)
                {

                }
            }
        }
    }
}