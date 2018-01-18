using Cstieg.ControllerHelper;
using DeerflyPatches.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DeerflyPatches
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Product",
                url: "Product/{id}",
                defaults: new { controller = "Home", action = "Product" }
            );

            routes.MapRoute(
                name: "Home",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { action = new IsHomeActionConstraint() }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }

    class IsHomeActionConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return ControllerHelper.HasAction(typeof(HomeController), values[parameterName].ToString());
        }
    }
}
