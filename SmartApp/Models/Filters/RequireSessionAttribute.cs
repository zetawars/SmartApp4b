
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Filters
{
    public class RequireSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller)filterContext.Controller;
            SessionUser _user = controller.Session["User"] as SessionUser;
            if (_user == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "action", "Login" }, { "controller", "Account" } });
            }
        }
    }
}