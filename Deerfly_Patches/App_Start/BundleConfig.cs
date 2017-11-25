using System.Web.Optimization;

namespace Deerfly_Patches
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
                      "~/Scripts/Deerfly_Patches/site.js",
                      "~/Scripts/Deerfly_Patches/gmaps.js",
                      "~/Scripts/Deerfly_Patches/inputmask.js",
                      "~/Scripts/Deerfly_Patches/ImageUpload.js",
                      "~/Scripts/Deerfly_Patches/LightboxMessage.js",
                      "~/Scripts/Deerfly_Patches/ShoppingCart.js",
                      "~/Scripts/Deerfly_Patches/EditIndex.js"));

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
