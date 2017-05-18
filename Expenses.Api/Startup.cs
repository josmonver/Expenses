using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(Expenses.Api.Startup))]
namespace Expenses.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            FilterConfig.RegisterGlobalFilters(config.Filters);
            RouteConfig.RegisterRoutes(config);
            ResponseFormatterConfig.ConfigJsonFormatter(config);

            var container = DependenciesConfig.RegisterDependencies(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseCors(CorsOptions.AllowAll);
            app.UseAutofacMiddleware(container);
            app.UseWebApi(config);
        }
    }
}