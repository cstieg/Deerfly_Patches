using Deerfly_Patches.Models;
using System.Web.Mvc;
using System.Data.Entity;
using Deerfly_Patches.Modules.Google;
using System.Linq;
using Deerfly_Patches.Modules;
using System.Threading.Tasks;
using System;
using Deerfly_Patches.Modules.Geography;

namespace Deerfly_Patches.Controllers
{
    public class RetailerMapController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private LatLng userLocation = null;

        // GET: RetailerMap
        public async Task<ActionResult> Index(string zip = "", int range = 50)
        {
            // If zip is passed, get coords for zip
            if (zip != "")
            {
                try
                {
                    var googleMapsClient = new GoogleMapsClient();
                    userLocation = await googleMapsClient.GeocodeAddress(zip);
                }
                catch
                {

                }
            }

            ViewBag.Location = userLocation;

            // Pass filter terms back to view
            ViewBag.Zip = zip;
            ViewBag.Range = range;
            // Zoom is based on search range
            ViewBag.Zoom = GoogleMapsClient.RadiusToZoom(range);
            ViewBag.GoogleMapsUrl = GoogleMapsClient.baseUrl + "js?key=" + GoogleMapsClient.apiKey + "&callback=initialMap";
            return View();
        }

        [HttpPost]
        public JsonResult SetUserLocation(float lat, float lng)
        {
            userLocation = new LatLng(lat, lng);
            return this.JOk();
        }

        public JsonResult UpdateRetailerJson(float maxLat, float leftLng, float minLat, float rightLng)
        {
            var data = GetRetailersInBounds(new GeoRange(maxLat, leftLng, minLat, rightLng));
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IQueryable<Retailer> GetRetailersInBounds(GeoRange range)
        {
            var retailers = db.Retailers.Include(r => r.LatLng).Include(r => r.Address)
                .Where(r => r.LatLng.Lat <= range.TopLeft.Lat &&
                            r.LatLng.Lng >= range.TopLeft.Lng &&
                            r.LatLng.Lat >= range.BottomRight.Lat &&
                            r.LatLng.Lng <= range.BottomRight.Lng);
            return retailers;
        }

    }
}