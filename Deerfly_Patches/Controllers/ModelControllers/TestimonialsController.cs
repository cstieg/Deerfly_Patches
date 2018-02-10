using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.WebFiles;
using Cstieg.WebFiles.Controllers;
using DeerflyPatches.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Testimonials
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    [RoutePrefix("edit/testimonials")]
    [Route("{action}/{id?}")]
    public class TestimonialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageManager imageSaver = new ImageManager("images/testimonials", new FileSystemService("/content"));

        // GET: Testimonials
        [Route("")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Testimonials.ToListAsync());
        }

        // GET: Testimonials/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Testimonial testimonial = await db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);
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

        /// <summary>
        /// Creates a new Testimonial model, saving an image to the default imageManager
        /// </summary>
        /// <param name="testimonial">The Testimonial model passed from the client</param>
        /// <returns>If valid POST, redirect to Testimonial Index; otherwise rerender the Create form</returns>
        // POST: Testimonials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TestimonialId,Label,Date,ImageUrl")] Testimonial testimonial)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, "");

            if (ModelState.IsValid)
            {
                // Save image to disk and store filepath in model
                try
                {
                    string timeStamp = FileManager.GetTimeStamp();
                    testimonial.ImageUrl = await imageSaver.SaveFile(imageFile, 200, timeStamp);
                    testimonial.ImageSrcSet = await imageSaver.SaveImageMultipleSizes(imageFile, new List<int>() { 1600, 800, 400, 200 }, timeStamp);
                }
                catch
                {
                    ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                    return View(testimonial);
                }

                // add new model
                db.Testimonials.Add(testimonial);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(testimonial);
        }

        // GET: Testimonials/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Testimonial testimonial = await db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        /// <summary>
        /// Edits a Testimonial model, saving an image to the default imageManager, and deleting the previous image
        /// </summary>
        /// <param name="testimonial">The Testimonial model passed from the client</param>
        /// <returns>If valid POST, redirect to Testimonial Index; otherwise rerender the Edit form</returns>
        // POST: Testimonials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TestimonialId,Label,Date,ImageUrl")] Testimonial testimonial)
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
                        string oldUrl = testimonial.ImageUrl;
                        string timeStamp = FileManager.GetTimeStamp();
                        testimonial.ImageUrl = await imageSaver.SaveFile(imageFile, 200, timeStamp);
                        testimonial.ImageSrcSet = await imageSaver.SaveImageMultipleSizes(imageFile, new List<int>() { 1600, 800, 400, 200, 100 }, timeStamp);
                        imageSaver.DeleteImageWithMultipleSizes(oldUrl);
                    }
                    catch
                    {
                        ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                        return View(testimonial);
                    }
                }

                // edit model
                db.Entry(testimonial).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Testimonial testimonial = await db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        /// <summary>
        /// Deletes a Testimonial model, along with the associated image files
        /// </summary>
        /// <param name="id">ID of Testimonial model to be deleted</param>
        /// <returns>Redirect to Testimonial Index</returns>
        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Testimonial testimonial = await db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);

            // Remove old image when deleting
            imageSaver.DeleteImageWithMultipleSizes(testimonial.ImageUrl);

            db.Testimonials.Remove(testimonial);
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
