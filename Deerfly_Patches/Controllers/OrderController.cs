using System.Linq;
using System.Web.Mvc;
using Deerfly_Patches.Models;


namespace Deerfly_Patches.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }
    }
}