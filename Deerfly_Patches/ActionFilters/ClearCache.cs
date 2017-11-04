using System;
using System.Web.Mvc;

namespace Deerfly_Patches.ActionFilters
{
    /// <summary>
    /// An ActionFilter to clear the cache after updating the data model so as to display fresh data when calling controller action
    /// </summary>
    public class ClearCache : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Cache.Insert("Pages", DateTime.Now);
            base.OnResultExecuted(filterContext);
        }

        /*
         * Global.asax.cs must contain:
        protected void Application_Start()
        {
            // Setup to clear cache
            HttpRuntime.Cache.Insert("Pages", DateTime.Now);
        }
         * Controllers (BaseController) must contain:  
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            filterContext.HttpContext.Response.AddCacheItemDependency("Pages");
        }
         */

    }
}