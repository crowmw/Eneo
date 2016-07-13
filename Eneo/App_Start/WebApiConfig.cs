using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Eneo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Api_Get",
                 routeTemplate: "{controller}/{action}/{id}",
           defaults: new { id = RouteParameter.Optional, area = "Eneo.Controllers.API" }
                //  constraints: new { httpMethod = new HttpMethodConstraint("GET") }
           );

            //config.Routes.MapHttpRoute(
            //   name: "Api_Post",
            //   routeTemplate: "api/{controller}/{action}/{id}",
            //   defaults: new { id = RouteParameter.Optional, action = "Post", area = "Eneo.Controllers.API" },
            //   constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            //);

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}