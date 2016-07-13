using Eneo.Site.App_Start;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Data.Entity;
using Eneo.Model;
using Eneo.Model.Migrations;

namespace Eneo.Site
{
    // Note: For instructions on enabling IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(SiteConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = false;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EneoContext, Configuration>());
            using (var temp = new EneoContext())
            {
                temp.Database.Initialize(true);
            }
        }
    }
}