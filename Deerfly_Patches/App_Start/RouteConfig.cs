using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Deerfly_Patches
{
    public class RouteConfig
    {
        public static string imagesFolder = "/content/images";
        public static string imagesPath = HostingEnvironment.MapPath("~" + imagesFolder);
        public static string productImagesFolder = imagesFolder + "/products";
        public static string productImagesPath = imagesPath + "/products";
        public static Boolean storeInCloud = true;
        

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
