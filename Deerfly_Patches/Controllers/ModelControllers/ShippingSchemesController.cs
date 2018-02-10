using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.Sales.Models;
using DeerflyPatches.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers.ModelControllers
{
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    [RoutePrefix("edit/shippingschemes")]
    [Route("{action}/{id?}")]
    public class ShippingSchemesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShippingSchemes
        [Route("")]
        public async Task<ActionResult> Index()
        {
            return View(await db.ShippingSchemes.ToListAsync());
        }

        // GET: ShippingSchemes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingScheme shippingScheme = await db.ShippingSchemes.FirstOrDefaultAsync(s => s.Id == id);
            if (shippingScheme == null)
            {
                return HttpNotFound();
            }
            return View(shippingScheme);
        }

        // GET: ShippingSchemes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShippingSchemes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description")] ShippingScheme shippingScheme)
        {
            if (ModelState.IsValid)
            {
                db.ShippingSchemes.Add(shippingScheme);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(shippingScheme);
        }

        // GET: ShippingSchemes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingScheme shippingScheme = await db.ShippingSchemes.FirstOrDefaultAsync(s => s.Id == id);
            if (shippingScheme == null)
            {
                return HttpNotFound();
            }
            return View(shippingScheme);
        }

        // POST: ShippingSchemes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description")] ShippingScheme shippingScheme)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingScheme).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shippingScheme);
        }

        // GET: ShippingSchemes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingScheme shippingScheme = await db.ShippingSchemes.FirstOrDefaultAsync(s => s.Id == id);
            if (shippingScheme == null)
            {
                return HttpNotFound();
            }
            return View(shippingScheme);
        }

        // POST: ShippingSchemes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShippingScheme shippingScheme = await db.ShippingSchemes.FirstOrDefaultAsync(s => s.Id == id);
            db.ShippingSchemes.Remove(shippingScheme);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
