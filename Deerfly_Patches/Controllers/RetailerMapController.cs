using Deerfly_Patches.Models;
using System.Web.Mvc;
using System.Data.Entity;
using Deerfly_Patches.Modules.Google;
using System.Linq;
using Deerfly_Patches.Modules;
using System.Threading.Tasks;
using System;

namespace Deerfly_Patches.Controllers
{
    public class RetailerMapController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RetailerMap
        public async Task<ActionResult> Index(string zip = "", int range = 50)
        {
            // Pass filter terms back to view
            ViewBag.Zip = zip;
            ViewBag.Range = range;

            LatLng location = null;

            // Try to get geolocation from entered zip, or IP
            try
            {
                if (zip == "")
                {
                    var geoLocation = await GeoLocation.GetGeoLocation();
                    location = geoLocation.LatLng;
                }
                else
                {
                    var googleMapsClient = new GoogleMapsClient();
                    location = await googleMapsClient.GeocodeAddress(zip);
                }
            }
            catch
            {
                location = new LatLng()
                {
                    Lat = 43,
                    Lng = -86
                };
            }

            ViewBag.Location = location;

            // Zoom is based on search range
            ViewBag.Zoom = GoogleMapsClient.RadiusToZoom(range);
            ViewBag.GoogleMapsUrl = GoogleMapsClient.baseUrl + "js?key=" + GoogleMapsClient.apiKey + "&callback=initialMap";

            // Haversine formula not able to be implemented in SQL due to requiring
            // cosines and arcsines.
            // Approximation with "flat earth" model also not possible due to requiring
            // square root.
            // Instead, filter assuming 1 degree of latitude or longitude is equal to 70 miles
            // Only return retailers located in square centered on location with a 
            // horizontal or vertical radius of given range.
            // This method may return results slightly beyond the given range, but will not
            // omit any results less than the given range.
            // Results may be further filtered on client side.
            var retailers = db.Retailers.Include(r => r.LatLng).Include(r => r.Address)
                .Where(r => Math.Abs(r.LatLng.Lat - location.Lat) * 70 <= range && 
                            Math.Abs(r.LatLng.Lng - location.Lng) * 70 <= range);

            return View(retailers);
        }
    }
}