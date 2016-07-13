using System.Web.Optimization;

namespace Eneo.Site
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/2015.1.318/jquery.min.js",
                "~/Scripts/kendo/2015.1.318/jszip.min.js",
                "~/Scripts/kendo/2015.1.318/kendo.all.min.js",
                "~/Scripts/kendo/2015.1.318/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/mapcontrol").Include(
                "~/Scripts/mapcontrol.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/css/bootstrap.icon-large.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/kendo").Include(
                "~/Content/kendo/2015.1.318/kendo.common.min.css",
                "~/Content/kendo/2015.1.318/kendo.mobile.all.min.css",
                "~/Content/kendo/2015.1.318/kendo.dataviz.min.css",
                "~/Content/kendo/2015.1.318/kendo.default.min.css",
                "~/Content/kendo/2015.1.318/kendo.dataviz.default.min.css"));
        }
    }
}