using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Expenses.Api
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                "DefaultApiWithId",
                "Api/{controller}/{id}",
                new { id = RouteParameter.Optional },
                new { id = @"\d+" });

            config.Routes.MapHttpRoute(
                "DefaultApiWithAction",
                "Api/{controller}/{action}");

            config.Routes.MapHttpRoute(
                "DefaultApiGet",
                "Api/{controller}",
                new { action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                "DefaultApiPost",
                "Api/{controller}",
                new { action = "Post" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
        }
    }
}