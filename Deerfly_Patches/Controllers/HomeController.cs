using Deerfly_Patches.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller for home page
    /// </summary>
    [OutputCache(CacheProfile = "CacheForADay")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            filterContext.HttpContext.Response.AddCacheItemDependency("Pages");
        }

        public async Task<ActionResult> Index()
        {
            var products = await db.Products.Where(p => p.DisplayOnFrontPage).ToListAsync();
            foreach (var product in products)
            {
                product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            }
            return View(products);
        }

        public ActionResult About()
        {
            return View();
        }

        public async Task<ActionResult> Product(int id)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            return View(product);
        }

        public ActionResult Contact()
        {
            return View();
        }

        public async Task<ActionResult> Testimonials()
        {
            var testimonials = await db.Testimonials.ToListAsync();
            return View(await db.Testimonials.ToListAsync());
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays list of links to model edit pages
        /// </summary>
        public ActionResult Edit()
        {
            string modelControllers = ConfigurationManager.AppSettings["modelControllers"];
            char[] delimiters = { ',' };
            string[] controllersArray = modelControllers.Split(delimiters);
            List<string> controllers = new List<string>(controllersArray);
            return View(controllers);
        }
    }
}