using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VsGallery.WebService.Formatting;

namespace VsGallery.WebService
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Insert(0, new TextMediaTypeFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{vsixId}",
                defaults: new { vsixId = RouteParameter.Optional }
            );
        }
    }
}
