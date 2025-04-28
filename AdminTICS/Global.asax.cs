using System;
using System.Web;
using System.Web.Http;

namespace AdminTICS
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
           
        }
    }
}
