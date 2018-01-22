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
                      "~/Scripts/Site/LightboxMessage.js",
                      "~/Scripts/Site/RandomId.js",
                      "~/Scripts/Site/ShoppingCart.js"));

            bundles.Add(new ScriptBundle("~/bundles/shoppingCart").Include(
                "~/Scripts/Site/PayPal.js"));

            bundles.Add(new ScriptBundle("~/bundles/retailers").Include(
                      "~/Scripts/Site/gmaps.js"));

            bundles.Add(new ScriptBundle("~/bundles/edit").Include(
                      "~/Scripts/NicEdit/nicEdit.js",
                      "~/Scripts/Site/inputmask.js",
                      "~/Scripts/Site/ImageUpload.js",
                      "~/Scripts/Sortable/sortable.js",
                      "~/Scripts/Site/EditIndex.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
