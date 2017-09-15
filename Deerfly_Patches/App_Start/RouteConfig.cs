using Deerfly_Patches.Modules.FileStorage;
using System.Web.Mvc;
using System.Web.Routing;

namespace Deerfly_Patches
{
    public class RouteConfig
    {
        // Set container for storage
        public static string contentFolder = "/content";
        public static IFileService storageService = new FileSystemService(contentFolder);

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
