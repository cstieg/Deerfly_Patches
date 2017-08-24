using Deerfly_Patches.Models;
using System.Web.Mvc;
using System.Data.Entity;
using Deerfly_Patches.Modules.Google;

namespace Deerfly_Patches.Controllers
{
    public class RetailerMapController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RetailerMap
        public ActionResult Index()
        {
            ViewBag.GoogleMapsUrl = GoogleMapsClient.baseUrl + "js?key=" + GoogleMapsClient.apiKey + "&callback=initialMap";
            var retailers = db.Retailers.Include(r => r.LatLng).Include(r => r.Address);
            return View(retailers);
        }
    }
}