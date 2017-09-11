using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules.Google;
using System.Threading.Tasks;

namespace Deerfly_Patches.Controllers.ModelControllers
{
    /// <summary>
    /// The controller providing model scaffolding for Retailers
    /// </summary>
    [Authorize]
    public class RetailersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Retailers
        public ActionResult Index()
        {
            var retailers = db.Retailers.Include(r => r.LatLng);
            return View(retailers.ToList());
        }

        // GET: Retailers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = db.Retailers.Find(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }
            return View(retailer);
        }

        // GET: Retailers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Retailers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RetailerId,Name,Address,Website")] Retailer retailer)
        {
            if (ModelState.IsValid)
            {
                // Geocode address of retailer
                retailer.LatLng = await new GoogleMapsClient().GeocodeAddress(retailer.Address);

                db.Retailers.Add(retailer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(retailer);
        }

        // GET: Retailers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = db.Retailers.Find(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }

            return View(retailer);
        }

        // POST: Retailers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RetailerId,Name,LatLngId")] Retailer retailer)
        {
            if (ModelState.IsValid)
            {
                // Geocode address of retailer
                retailer.LatLng = await new GoogleMapsClient().GeocodeAddress(retailer.Address);

                db.Entry(retailer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(retailer);
        }

        // GET: Retailers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = db.Retailers.Find(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }
            return View(retailer);
        }

        // POST: Retailers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Retailer retailer = db.Retailers.Find(id);
            db.Retailers.Remove(retailer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult ListJson()
        {
            //TODO: filter by location
            var retailers = db.Retailers.Include(r => r.LatLng);
            var returnval = retailers.ToList();
            return Json(returnval, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
