using System;
using System.Linq;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules.PayPal;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller for Order page
    /// </summary>
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public ActionResult Index()
        {
            return View(db.Products.Where(p => p.DoNotDisplay == false).ToList());
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response if successful, error response if product already exists</returns>
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
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Add new order detail to session
            try
            {
                shoppingCart.AddProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(e.Message);
            }
        }

        /// <summary>
        /// Increases the quantity of a product in the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncrementItemInShoppingCart(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Increment quantity and save shopping cart
            try
            {
                shoppingCart.IncrementProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(e.Message);
            }

        }

        /// <summary>
        /// Decreases the quantity of an item in the shopping cart
        /// </summary>
        /// <param name="id">ID of the Product model qty to decrement</param>
        /// <returns>JSON success response</returns>
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
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Decrement qty and update shopping cart in session
            try
            {
                shoppingCart.DecrementProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(e.Message);
            }

        }

        /// <summary>
        /// Removes a Product from the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to remove</param>
        /// <returns>JSON success response</returns>
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
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Remove Product and update shopping cart in session
            try
            {
                shoppingCart.RemoveProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(e.Message);
            }

        }

    }
}