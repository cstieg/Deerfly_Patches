using System.Web.Optimization;

namespace DeerflyPatches
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Scripts/Site/site.js",
                      "~/Scripts/Site/gmaps.js",
                      "~/Scripts/Site/inputmask.js",
                      "~/Scripts/Site/ImageUpload.js",
                      "~/Scripts/Site/LightboxMessage.js",
                      "~/Scripts/Site/ShoppingCart.js",
                      "~/Scripts/Site/EditIndex.js"));

            bundles.Add(new ScriptBundle("~/bundles/nicedit").Include(
                      "~/Scripts/NicEdit/nicEdit.js"));

            bundles.Add(new ScriptBundle("~/bundles/sortable").Include(
                      "~/Scripts/Sortable/sortable.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
