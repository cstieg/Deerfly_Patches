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
            // Pass filter terms back to view
            ViewBag.Zip = zip;
            ViewBag.Range = range;

            if (userLocation == null)
            {

                // Try to get geolocation from entered zip, or IP
                try
                {
                    if (zip == "")
                    {
                        var geoLocation = await GeoLocation.GetGeoLocation();
                        userLocation = geoLocation.LatLng;
                    }
                    else
                    {
                        var googleMapsClient = new GoogleMapsClient();
                        userLocation = await googleMapsClient.GeocodeAddress(zip);
                    }
                }
                catch
                {
                    userLocation = new LatLng(43, -86);
                }
            }


            ViewBag.Location = userLocation;

            // Zoom is based on search range
            ViewBag.Zoom = GoogleMapsClient.RadiusToZoom(range);
            ViewBag.GoogleMapsUrl = GoogleMapsClient.baseUrl + "js?key=" + GoogleMapsClient.apiKey + "&callback=initialMap";


            var retailers = GetRetailersInBounds(GeoLocation.GetGeoRange(userLocation, range));
            return View(retailers);
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