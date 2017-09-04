using System.Linq;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.ViewModels;
using Deerfly_Patches.Modules;

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
            return View(db.Products.ToList());
        }

        /// <summary>
        /// Adds a product to the shopping cart, or increments if already present
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response</returns>
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
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            // Create new shopping cart if none is in session
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }

            // Decrement qty and update shopping cart in session
            shoppingCart.DecrementProduct(product);
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);
            return this.JOk();
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
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            // Create new shopping cart if none is in session
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }

            // Remove Product and update shopping cart in session
            shoppingCart.RemoveProduct(product);
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);
            return this.JOk();
        }

    }
}