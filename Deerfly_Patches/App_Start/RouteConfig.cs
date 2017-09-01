using System;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Deerfly_Patches
{
    public class RouteConfig
    {
        public static string contentFolder = "/content";
        public static string storageService = "fileSystem";
        

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
