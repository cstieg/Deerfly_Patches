using System.Web.Optimization;

namespace DeerflyPatches
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/Site/site.js",
                      "~/Scripts/Site/LightboxMessage.js",
                      "~/Scripts/Site/RandomId.js",
                      "~/Scripts/Site/ShoppingCart.js"));

            bundles.Add(new ScriptBundle("~/bundles/shoppingCart").Include(
                      "~/Scripts/Site/ShoppingCartCountry.js",
                      "~/Scripts/Site/PayPal.js",
                      "~/Scripts/Site/TextareaPattern.js"));

            bundles.Add(new ScriptBundle("~/bundles/retailers").Include(
                      "~/Scripts/Site/gmaps.js"));

            bundles.Add(new ScriptBundle("~/bundles/edit").Include(
                      "~/Scripts/NicEdit/nicEdit.js",
                      "~/Scripts/Site/ImageUpload.js",
                      "~/Scripts/Sortable/sortable.js",
                      "~/Scripts/Site/EditIndex.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/ShoppingCart.css",
                      "~/Content/LightboxMessage.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
