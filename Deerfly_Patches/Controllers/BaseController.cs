using Cstieg.WebFiles;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Base controller to be provide basic behavior for all controllers
    /// </summary>
    public class BaseController : Controller
    {
        public static string contentFolder = "/content";

        // storage service for storing uploaded images
        protected IFileService storageService;
        protected ImageManager imageManager;

        public BaseController()
        {
            // Set storage service for product images
            storageService = new FileSystemService(contentFolder);
            imageManager = new ImageManager("images/products", storageService);
        }

        /// <summary>
        /// Add dependency to cache so it is refreshed when updating the dependency
        /// </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            filterContext.HttpContext.Response.AddCacheItemDependency("Pages");
        }
    }
}