using Microsoft.Owin;
using Owin;
using SmartApp.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


[assembly: OwinStartup("Startup", typeof(Startup))]
namespace SmartApp.Hubs
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}

