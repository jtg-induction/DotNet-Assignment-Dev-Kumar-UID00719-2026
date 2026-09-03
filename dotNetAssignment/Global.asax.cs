using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace dotNetAssignment
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// This method is called when the application starts. It configures the Web API routes and other settings.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
