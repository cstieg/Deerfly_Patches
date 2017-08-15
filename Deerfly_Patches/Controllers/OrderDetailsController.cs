using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.ViewModels;
using Deerfly_Patches.Modules;

namespace Deerfly_Patches.Controllers
{
    public class OrderDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            return View(orderDetails.ToList());
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "OrderId");
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderDetailId,ProductId,PlacedInCart,Quantity,UnitPrice,Shipping,CheckedOut,OrderId")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderDetailId,ProductId,PlacedInCart,Quantity,UnitPrice,Shipping,CheckedOut,OrderId")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
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

        private bool OrderDetailExists(int id)
        {
            return db.OrderDetails.Any(e => e.OrderDetailId == id);
        }


        [HttpPost, ActionName("AddOrderDetailToShoppingCart")]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrderDetailToShoppingCart(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            // Create new shopping cart if none is in session
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart();
            }

            // Add new order detail to session
            shoppingCart.AddProduct(product);
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);
            return this.JOk();
        }

        [HttpPost, ActionName("DecrementItemInShoppingCart")]
        [ValidateAntiForgeryToken]
        public ActionResult DecrementItemInShoppingCart(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            // Create new shopping cart if none is in session
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }

            // Add new order detail to session
            shoppingCart.DecrementProduct(product);
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);
            return this.JOk();
        }


        [HttpPost, ActionName("RemoveItemInShoppingCart")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItemInShoppingCart(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            // Create new shopping cart if none is in session
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }

            // Add new order detail to session
            shoppingCart.RemoveProduct(product);
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);
            return this.JOk();
        }

    }
}
