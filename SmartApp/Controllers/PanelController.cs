
using SmartApp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartApp.Controllers
{

  //  [RequireSession]
    public class PanelController : BaseController
    {
        public SessionUser user { get { return Session["User"] as SessionUser; } }

    }
}