using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace AdminTICS
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var routes = GlobalConfiguration.Configuration.Routes;
            foreach (var route in routes)
            {
                Console.WriteLine(route.RouteTemplate);
            }

        }
    }
}
