
using SmartApp.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Filters
{
    public class ErrorHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            var controller = (BaseController)filterContext.Controller;
            var isAjaxRequest = filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();
            var errorMessages = new StringBuilder();
            if (filterContext.Exception.GetType() == typeof(SqlException))
            {
                var ex = (SqlException)filterContext.Exception;
                for (var i = 0; i < ex.Errors.Count; i++)
                {
                    string message = $@"
 Message: { ex.Errors[i].Message} Error Number: {ex.Errors[i].Number} LineNumber: {ex.Errors[i].LineNumber} Source: {ex.Errors[i].Source}
";

                    errorMessages.Append(message);
                }
                ShowSqlExceptionMessage(ex.Number, controller, isAjaxRequest);

            } else
            {
                var exception = filterContext.Exception;
                ShowOtherExceptionMessage(exception.Message, controller, isAjaxRequest);
            }

            if (!isAjaxRequest)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "action", "Error500" }, { "controller", "Error" } });
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "action", "AjaxRedirect" }, { "controller", "Error" } });
            }
        }

        internal void ShowOtherExceptionMessage(string message, BaseController controller, bool isAjaxRequest)
        {
            controller.Notify(new NotificationMessage
            {
                Code = "500",
                Description = $"Internal Server Error: 500. <br/> {message} <br/> Please contact the Administrator.",
                Title = "Technical Error",
                MessageType = "Error",
                IsViewMessage = false,
                IsAjaxMessage = isAjaxRequest,
                IsRedirectMessage = !isAjaxRequest
            });
        }

        internal void ShowSqlExceptionMessage(int code, BaseController controller, bool isAjaxRequest)
        {
            controller.Notify(new NotificationMessage
            {
                Code = code.ToString(),
                Description = $"Internal Server Error: {code}. <br/> Please contact the Administrator.",
                Title = "Technical Error",
                MessageType = "Error",
                IsViewMessage = false,
                IsAjaxMessage = isAjaxRequest,
                IsRedirectMessage = !isAjaxRequest
            });
        }
    }


    //public class ErrorHandler : FilterAttribute, IExceptionFilter
    //{
    //    public void OnException(ExceptionContext filterContext)
    //    {
    //        filterContext.ExceptionHandled = true;
    //        BaseController controller = (BaseController)filterContext.Controller;
    //        controller.Notify(new NotificationMessage
    //        {
    //            Code = filterContext.Exception.HResult.ToString(),
    //            Description = filterContext.Exception.Message.ToString(),
    //            IsAjaxMessage = true,
    //            IsViewMessage = true,
    //            IsRedirectMessage = true,
    //            Title = "Technical Error",
    //            MessageType = "Error"
    //        });
    //        //controller.log.Error("Technical Error: " + filterContext.Exception.Message.ToString() + "Page :" + filterContext.HttpContext.Request.Url + ", Previous Page :" + filterContext.HttpContext.Request.UrlReferrer);
    //    }
    //}
}