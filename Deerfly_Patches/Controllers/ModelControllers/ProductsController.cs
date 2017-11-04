using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Cstieg.WebFiles.Controllers;
using Cstieg.WebFiles;
using Deerfly_Patches.ActionFilters;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Products
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static string contentFolder = "/content";

        protected IFileService storageService;
        protected ImageManager imageManager;

        public ProductsController()
        {
            storageService = new FileSystemService(contentFolder);
            imageManager = new ImageManager("images/products", storageService);
        }

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
        public async Task<ActionResult> Create()
        {
            // delete images that were previously saved to newly created product that was not ultimately saved
            foreach (var webImage in await db.WebImages.Where(w => w.ProductId == null).ToListAsync())
            {
                // remove image files used by product
                imageManager.DeleteImageWithMultipleSizes(webImage.ImageUrl);

                db.WebImages.Remove(webImage);
                await db.SaveChangesAsync();
            }

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
        public async Task<ActionResult> Create([Bind(Include = "ProductId,Name,Price,Shipping,DisplayOnFrontPage,DoNotDisplay,ProductInfo")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();

                // connect images that were previously saved to product (id = null)
                foreach (var webImage in await db.WebImages.Where(w => w.ProductId == null).ToListAsync())
                {
                    webImage.ProductId = product.ProductId;
                    db.Entry(webImage).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

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

            // Pass in list of images for product
            product.WebImages = product.WebImages ?? new List<WebImage>();

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
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,Name,Price,Shipping,DisplayOnFrontPage,DoNotDisplay,ProductInfo")] Product product)
        {
            if (ModelState.IsValid)
            {
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

            // Delete images connected to this product
            foreach (var webImage in await db.WebImages.Where(w => w.ProductId == product.ProductId).ToListAsync())
            {
                // remove image files used by product
                imageManager.DeleteImageWithMultipleSizes(webImage.ImageUrl);

                db.WebImages.Remove(webImage);
                await db.SaveChangesAsync();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Adds an image to the product model
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Json result containing image id</returns>
        [HttpPost]
        public async Task<JsonResult> AddImage(int? id)
        {
            if (id != null)
            {
                Product product = await db.Products.FindAsync(id);
                if (product == null)
                {
                    return this.JError(404, "Can't find product " + id.ToString());
                }
            }


            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, "", "file");

            // Save image to disk and store filepath in model
            try
            {
                string timeStamp = FileManager.GetTimeStamp();
                WebImage image = new WebImage
                {
                    ProductId = id,
                    ImageUrl = await imageManager.SaveFile(imageFile, 200, timeStamp),
                    ImageSrcSet = await imageManager.SaveImageMultipleSizes(imageFile, new List<int>() { 800, 400, 200, 100 }, timeStamp)
                };
                db.WebImages.Add(image);
                await db.SaveChangesAsync();
                return new JsonResult
                {
                    Data = new
                    {
                        success = "True",
                        imageId = image.Id
                    }
                };
            }
            catch (Exception e)
            {
                return this.JError(400, "Error saving image: " + e.Message);
            }
        }

        /// <summary>
        /// Deletes an image from the product model
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Json result containing image id</returns>
        [HttpPost]
        public async Task<JsonResult> DeleteImage(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return this.JError(404, "Can't find product " + id.ToString());
            }

            int imageId = int.Parse(Request.Params.Get("imageId"));
            WebImage image = await db.WebImages.FindAsync(imageId);
            if (image == null)
            {
                return this.JError(404, "Can't find image " + imageId.ToString());
            }

            // remove image files used by product
            imageManager.DeleteImageWithMultipleSizes(image.ImageUrl);

            db.WebImages.Remove(image);
            await db.SaveChangesAsync();
            return new JsonResult
            {
                Data = new
                {
                    success = "True",
                    imageId = image.Id
                }
            };
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
