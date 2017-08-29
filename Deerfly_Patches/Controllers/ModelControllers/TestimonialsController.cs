using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules;

namespace Deerfly_Patches.Controllers.ModelControllers
{
    public class TestimonialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string[] validImageTypes = new string[]
        {
            "image/gif",
            "image/jpeg",
            "image/png"
        };


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
            HttpPostedFileBase imageFile = GetImageFile(ModelState, Request);

            if (ModelState.IsValid)
            {
                // Save image to disk and store filepath in model
                string urlPath = new FileStorage().SaveImage(imageFile);
                if (urlPath != "")
                {
                    testimonial.ImageUrl = urlPath;
                }

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
            HttpPostedFileBase imageFile = GetImageFile(ModelState, Request);

            if (ModelState.IsValid)
            {
                // Save image to disk and store filepath in model
                string urlPath = new FileStorage().SaveImage(imageFile);
                if (urlPath != "")
                {
                    testimonial.ImageUrl = urlPath;
                }

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

        private HttpPostedFileBase GetImageFile(ModelStateDictionary ModelState, HttpRequestBase Request)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = null;
            if (Request.Files.Count == 0)
            {
                ModelState.AddModelError("ImageUrl", "This field is required");
            }
            else
            {
                imageFile = Request.Files[0];
            }

            if (imageFile != null && (imageFile.ContentLength == 0 || !validImageTypes.Contains(imageFile.ContentType)))
            {
                ModelState.AddModelError("ImageUrl", "Please choose either a valid GIF, JPG or PNG image.");
            }

            return imageFile;
        }

    }
}
