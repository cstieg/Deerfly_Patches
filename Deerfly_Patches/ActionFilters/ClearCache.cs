using System.Collections;
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
            base.OnResultExecuted(filterContext);
            var context = filterContext.HttpContext;

            context.Response.RemoveOutputCacheItem("/");
            // remove each item from cache
            foreach (DictionaryEntry item in context.Cache)
            {
                context.Cache.Remove(item.Key as string);
            }
        }
    }
}