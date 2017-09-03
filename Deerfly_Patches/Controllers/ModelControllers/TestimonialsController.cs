﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules.FileStorage;
using System.Collections.Generic;

namespace Deerfly_Patches.Controllers.ModelControllers
{
    public class TestimonialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageSaver imageSaver = new ImageSaver("images/testimonials");

        // GET: Testimonials
        public ActionResult Index()
        {
            return View(db.Testimonials.ToList());
        }

        // GET: Testimonials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        // GET: Testimonials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Testimonials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestimonialId,Label,Date,ImageUrl")] Testimonial testimonial)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, "");

            if (ModelState.IsValid)
            {
                // Save image to disk and store filepath in model
                try
                {
                    testimonial.ImageUrl = imageSaver.SaveFile(imageFile, 1600);
                    testimonial.ImageSrcSet = imageSaver.SaveImageMultipleSizes(imageFile, new List<int>() { 1600, 800, 400, 200, 100 });
                }
                catch
                {
                    ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                    return View(testimonial);
                }

                // add new model
                db.Testimonials.Add(testimonial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testimonial);
        }

        // GET: Testimonials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        // POST: Testimonials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestimonialId,Label,Date,ImageUrl")] Testimonial testimonial)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, testimonial.ImageUrl);

            if (ModelState.IsValid)
            {
                // imageFile is null if no file was uploaded, but previous file exists
                if (imageFile != null)
                {
                    // Save image to disk and store filepath in model
                    try
                    {
                        testimonial.ImageUrl = imageSaver.SaveFile(imageFile, 1600);
                        testimonial.ImageSrcSet = imageSaver.SaveImageMultipleSizes(imageFile, new List<int>() { 1600, 800, 400, 200, 100 });
                    }
                    catch
                    {
                        ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                        return View(testimonial);
                    }
                }

                // edit model
                db.Entry(testimonial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Testimonial testimonial = db.Testimonials.Find(id);
            db.Testimonials.Remove(testimonial);
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
