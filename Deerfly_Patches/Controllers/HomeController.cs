using Deerfly_Patches.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller for home page
    /// </summary>
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Products.Where(p => p.DoNotDisplay == false).ToListAsync());
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public async Task<ActionResult> Testimonials()
        {
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