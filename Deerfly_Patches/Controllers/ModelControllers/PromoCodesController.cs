using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using System.Threading.Tasks;
using Deerfly_Patches.ActionFilters;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for PromoCodes
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    public class PromoCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PromoCodes
        public async Task<ActionResult> Index()
        {
            var promoCodes = db.PromoCodes.Include(p => p.PromotionalItem).Include(p => p.WithPurchaseOf);
            return View(await promoCodes.ToListAsync());
        }

        // GET: PromoCodes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            PromoCode promoCode = await db.PromoCodes.FindAsync(id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            return View(promoCode);
        }

        // GET: PromoCodes/Create
        public ActionResult Create()
        {
            ViewBag.PromotionalItemId = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.WithPurchaseOfId = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.SpecialPriceItemId = new SelectList(db.Products, "ProductId", "Name");
            return View();
        }

        // POST: PromoCodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                db.PromoCodes.Add(promoCode);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PromotionalItemId = new SelectList(db.Products, "ProductId", "Name", promoCode.PromotionalItemId);
            ViewBag.WithPurchaseOfId = new SelectList(db.Products, "ProductId", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemId = new SelectList(db.Products, "ProductId", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // GET: PromoCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            PromoCode promoCode = await db.PromoCodes.FindAsync(id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.PromotionalItemList = new SelectList(db.Products, "ProductId", "Name", promoCode.PromotionalItemId);
            ViewBag.WithPurchaseOfIdList = new SelectList(db.Products, "ProductId", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemIdList = new SelectList(db.Products, "ProductId", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // POST: PromoCodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promoCode).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PromotionalItemId = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.WithPurchaseOfId = new SelectList(db.Products, "ProductId", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemId = new SelectList(db.Products, "ProductId", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // GET: PromoCodes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            PromoCode promoCode = await db.PromoCodes.FindAsync(id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            return View(promoCode);
        }

        // POST: PromoCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PromoCode promoCode = await db.PromoCodes.FindAsync(id);
            db.PromoCodes.Remove(promoCode);
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
