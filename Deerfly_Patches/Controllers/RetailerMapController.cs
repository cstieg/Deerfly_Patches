using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Deerfly_Patches.Models;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.Geography;
using Deerfly_Patches.Modules.Google;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller to display retailer map
    /// </summary>
    public class RetailerMapController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private LatLng userLocation = null;

        /// <summary>
        /// Gets the retailer map page for a particular zip and mile range
        /// </summary>
        /// <param name="zip">The zip of the user, on which to center the map</param>
        /// <param name="range">The range in miles to find retailers within</param>
        /// <returns>The retailer map view</returns>
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
                    userLocation = new LatLng(43, -86);
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

        /// <summary>
        /// AJAX request to update user location
        /// </summary>
        /// <param name="lat">Latitute of the user</param>
        /// <param name="lng">Longitude of the user</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        public JsonResult SetUserLocation(float lat, float lng)
        {
            userLocation = new LatLng(lat, lng);
            return this.JOk();
        }

        /// <summary>
        /// AJAX request to get retailers within a geographical range
        /// </summary>
        /// <param name="maxLat">Maximum (northernmost) latitude of range</param>
        /// <param name="leftLng">Minimum (westernmost) longitude of range</param>
        /// <param name="minLat">Minimum (southernmost) latitude of range</param>
        /// <param name="rightLng">Maximum (easternmost) longitude of range</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UpdateRetailerJson(float maxLat, float leftLng, float minLat, float rightLng)
        {
            var data = GetRetailersInBounds(new GeoRange(maxLat, leftLng, minLat, rightLng));
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /// <summary>
        /// Helper method to get retailers within range from model
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
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