using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.Sales.Models;
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
        // GET: /
        public async Task<ActionResult> Index()
        {
            var products = await _context.Products.Where(p => p.DisplayOnFrontPage).ToListAsync();
            foreach (var product in products)
            {
                product.WebImages = product.WebImages?.OrderBy(w => w.Order).ToList();
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
                if (product.WebImages != null)
                {
                    product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
                }
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
                return await ProductByUrlName(id);
            }
        }
        
        // GET: ProductById/1
        public async Task<ActionResult> ProductById(int id)
        {
            string circumferenceParam = Request.Params.Get("circumference");
            string unit = Request.Params.Get("unit");
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            return View("Product", product);
        }

        // GET: ProductById/Product Name
        public async Task<ActionResult> ProductByUrlName(string urlName)
        {
            Product product = await _context.Products.Where(p => p.UrlName != null && p.UrlName.ToLower() == urlName.ToLower()).SingleOrDefaultAsync();
            if (product == null)
            {
                return HttpNotFound();
            }

            if (product.WebImages != null)
            {
                product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            }
            return View("Product", product);
        }

        public ActionResult Contact()
        {
            return View();
        }

        public async Task<ActionResult> Testimonials()
        {
            var testimonials = await _context.Testimonials.ToListAsync();
            return View(await _context.Testimonials.ToListAsync());
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Returns()
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

        [ClearCache]
        public ActionResult ClearCache()
        {
            return RedirectToActionPermanent("Index");
        }
    }
}