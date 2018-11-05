
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amourss.Filters
{
    //public class ValidatorAttribute : ActionFilterAttribute
    //{
    //    public WebSettingsRepository WebRepo { get; set; }

    //    public ValidatorAttribute()
    //    {
    //        this.WebRepo = new WebSettingsRepository();
    //    }
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var controller = (PanelController)filterContext.Controller;
    //        string actionName = filterContext.ActionDescriptor.ActionName;
    //        string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
    //        if (WebRepo.DateExpired() && filterContext.ActionDescriptor.ActionName != "Expired" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Error") 
    //        {
    //            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "action", "Expired" }, { "controller", "Error" } });
    //        }
    //    }
    //}
}