using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AdminTICS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Habilitar CORS globalmente
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);


            // Configurar rutas de Web API
            config.MapHttpAttributeRoutes();

            //config.MessageHandlers.Add(new PreflightRequestsHandler());

            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
);
        }
    }
}
