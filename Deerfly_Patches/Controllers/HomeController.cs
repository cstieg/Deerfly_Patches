using DeerflyPatches.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller for home page
    /// </summary>
    [OutputCache(CacheProfile = "CacheForAWeek")]
    public class HomeController : BaseController
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
        
        // GET: Products
        public async Task<ActionResult> Products()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var products = await db.Products.Where(p => !p.DoNotDisplay).ToListAsync();
            foreach (var product in products)
            {
                product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            }
            return View(products);
        }

        // GET: Product/1
        // GET: Product/Product Name
        /// <summary>
        /// Routes product page request to the appropriate controller method
        /// </summary>
        /// <param name="id">May be an integer id or string product name</param>
        public async Task<ActionResult> Product(string id)
        {
            try
            {
                return await ProductById(int.Parse(id));
            }
            catch
            {
                return await ProductByProductName(id);
            }
        }
        
        // GET: ProductById/1
        public async Task<ActionResult> ProductById(int id)
        {
            string circumferenceParam = Request.Params.Get("circumference");
            string unit = Request.Params.Get("unit");
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            return View("Product", product);
        }

        // GET: ProductById/Product Name
        public async Task<ActionResult> ProductByProductName(string productName)
        {
            Product product = await db.Products.Where(p => p.Name.ToLower() == productName.ToLower()).SingleOrDefaultAsync();
            if (product == null)
            {
                return HttpNotFound();
            }

            product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            return View("Product", product);
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