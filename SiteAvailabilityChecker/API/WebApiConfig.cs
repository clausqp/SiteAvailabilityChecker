using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SiteAvailabilityChecker.API
{
    internal class WebApiConfig
    {


        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "ApiV1",
                routeTemplate: "api/v1/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "ApiV1", action = "GetVersion" }
            );

            // Web Api
            app.UseWebApi(config);
        }


    }
}
