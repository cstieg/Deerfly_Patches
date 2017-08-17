using System.Linq;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.ViewModels;
using Deerfly_Patches.Modules;
using System.Web;

namespace Deerfly_Patches.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public ActionResult Index()
        {
            return View(db.Products.ToList());
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