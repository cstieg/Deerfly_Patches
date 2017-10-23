using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using System.Web;
using Deerfly_Patches.Modules.FileStorage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Products
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageManager imageManager = new ImageManager("images/products");

        // GET: Products
        public async Task<ActionResult> Index()
        {
            return View(await db.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new Product model, saving an image to the default imageManager
        /// </summary>
        /// <param name="product">The Product model passed from the client</param>
        /// <returns>If valid POST, redirect to Product Index; otherwise rerender the Create form</returns>
        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,Name,Description,Price,Shipping,ImageUrl,Category,DisplayOnFrontPage,DoNotDisplay")] Product product)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, "");

            if (ModelState.IsValid)
            {
                // Save image to disk and store filepath in model
                try
                {
                    string timeStamp = FileManager.GetTimeStamp();
                    product.ImageUrl = imageManager.SaveFile(imageFile, 200, true, timeStamp);
                    product.ImageSrcSet = imageManager.SaveImageMultipleSizes(imageFile, new List<int>() { 800, 400, 200, 100 }, true, timeStamp);
                }
                catch
                {
                    ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                    return View(product);
                }

                // add new model
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        /// <summary>
        /// Edits a Product model, saving a new image to the default imageManager, and deleting the old
        /// </summary>
        /// <param name="product">The Product model passed from the POST request</param>
        /// <returns>If valid POST, redirect to Product Index; otherwise rerender the Edit form</returns>
        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,Name,Description,Price,Shipping,ImageUrl,Category,DisplayOnFrontPage,DoNotDisplay")] Product product)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, product.ImageUrl);

            if (ModelState.IsValid)
            {
                // imageFile is null if no file was uploaded, but previous file exists
                if (imageFile != null)
                {
                    // Save image to disk and store filepath in model
                    try
                    {
                        string oldUrl = product.ImageUrl;
                        string timeStamp = FileManager.GetTimeStamp();
                        product.ImageUrl = imageManager.SaveFile(imageFile, 200, true, timeStamp);
                        product.ImageSrcSet = imageManager.SaveImageMultipleSizes(imageFile, new List<int>() { 800, 400, 200, 100 }, true, timeStamp);
                        imageManager.DeleteImageWithMultipleSizes(oldUrl);
                    }
                    catch
                    {
                        ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                        return View(product);
                    }
                }

                // edit model
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        /// <summary>
        /// Deletes a Product model, along with the associated image files
        /// </summary>
        /// <param name="id">ID of Product model to be deleted</param>
        /// <returns>Redirect to Product Index</returns>
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);

            // remove image files used by product
            imageManager.DeleteImageWithMultipleSizes(product.ImageUrl);

            db.Products.Remove(product);
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
