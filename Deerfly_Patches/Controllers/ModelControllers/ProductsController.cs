using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using System.Web;
using Deerfly_Patches.Modules.FileStorage;
using System;
using System.Collections.Generic;

namespace Deerfly_Patches.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageSaver imageSaver = new ImageSaver("images/products");
        private string[] validImageTypes = new string[]
        {
            "image/gif",
            "image/jpeg",
            "image/png"
        };

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
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

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Name,Description,Price,Shipping,ImageUrl,Category,DisplayOnFrontPage,PayPalUrl")] Product product)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = GetImageFile(ModelState, Request, "");

            if (ModelState.IsValid)
            {
                // Save image to disk and store filepath in model
                try
                {
                    product.ImageUrl = imageSaver.SaveFile(imageFile, 200);
                    product.ImageSrcSet = imageSaver.SaveImageMultipleSizes(imageFile, new List<int>() { 800, 400, 200, 100 });
                }
                catch
                {
                    ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                    return View(product);
                }

                // add new model
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Description,Price,Shipping,ImageUrl,Category,DisplayOnFrontPage,PayPalUrl")] Product product)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = GetImageFile(ModelState, Request, product.ImageUrl);

            if (ModelState.IsValid)
            {
                // imageFile is null if no file was uploaded, but previous file exists
                if (imageFile != null)
                {
                    // Save image to disk and store filepath in model
                    try
                    {
                        product.ImageUrl = imageSaver.SaveFile(imageFile, 200);
                        product.ImageSrcSet = imageSaver.SaveImageMultipleSizes(imageFile, new List<int>() { 800, 400, 200, 100 });
                    }
                    catch
                    {
                        ModelState.AddModelError("ImageUrl", "Failure saving image. Please try again.");
                        return View(product);
                    }
                }

                // edit model
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        private HttpPostedFileBase GetImageFile(ModelStateDictionary ModelState, HttpRequestBase Request, string imageUrl)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = null;

            if (Request.Files.Count == 0)
            {
                // Don't add model error if there is already an image file
                if (!imageUrl.Equals(""))
                {
                    return null;
                }

                ModelState.AddModelError("ImageUrl", "This field is required");
            }
            else
            {
                imageFile = Request.Files[0];
            }

            if (imageFile != null && (imageFile.ContentLength == 0 || !validImageTypes.Contains(imageFile.ContentType)))
            {
                // Don't add model error if there is already an image file
                if (!imageUrl.Equals(""))
                {
                    return null;
                }

                ModelState.AddModelError("ImageUrl", "Please choose either a valid GIF, JPG or PNG image.");
            }

            return imageFile;
        }

    }
}
