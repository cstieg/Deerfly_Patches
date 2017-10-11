﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for PromoCodes
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class PromoCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PromoCodes
        public ActionResult Index()
        {
            var promoCodes = db.PromoCodes.Include(p => p.PromotionalItem).Include(p => p.WithPurchaseOf);
            return View(promoCodes.ToList());
        }

        // GET: PromoCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromoCode promoCode = db.PromoCodes.Find(id);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                db.PromoCodes.Add(promoCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PromotionalItemId = new SelectList(db.Products, "ProductId", "Name", promoCode.PromotionalItemId);
            ViewBag.WithPurchaseOfId = new SelectList(db.Products, "ProductId", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemId = new SelectList(db.Products, "ProductId", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // GET: PromoCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromoCode promoCode = db.PromoCodes.Find(id);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promoCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PromotionalItemId = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.WithPurchaseOfId = new SelectList(db.Products, "ProductId", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemId = new SelectList(db.Products, "ProductId", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // GET: PromoCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromoCode promoCode = db.PromoCodes.Find(id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            return View(promoCode);
        }

        // POST: PromoCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PromoCode promoCode = db.PromoCodes.Find(id);
            db.PromoCodes.Remove(promoCode);
            db.SaveChanges();
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
