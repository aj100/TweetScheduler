using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace TweetSchedulerClient
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var mvcRoutes = RouteTable.Routes;
            mvcRoutes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            mvcRoutes.MapRoute("AuthRoute", "authenticate/{action}", new { controller = "Auth", action = "BeginAuth" });
            mvcRoutes.MapRoute("Default", "{controller}/{action}", new { controller = "Main", action = "Main" });
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}