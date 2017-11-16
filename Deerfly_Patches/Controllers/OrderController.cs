using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules.PayPal;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller for Order page
    /// </summary>
    [OutputCache(CacheProfile = "CacheForADay")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order
        public async Task<ActionResult> Index()
        {
            return View(await db.Products.Where(p => p.DoNotDisplay == false).ToListAsync());
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response if successful, error response if product already exists</returns>
        [HttpPost, ActionName("AddOrderDetailToShoppingCart")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrderDetailToShoppingCart(int id)
        {
            // look up product entity
            Product product = await db.Products.Include(p => p.WebImages).SingleOrDefaultAsync(m => m.ProductId == id);
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
                return this.JError(403, e.Message);
            }
        }

        /// <summary>
        /// Increases the quantity of a product in the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IncrementItemInShoppingCart(int id)
        {
            // look up product entity
            Product product = await db.Products.SingleOrDefaultAsync(m => m.ProductId == id);
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
                return this.JError(403, e.Message);
            }

        }

        /// <summary>
        /// Decreases the quantity of an item in the shopping cart
        /// </summary>
        /// <param name="id">ID of the Product model qty to decrement</param>
        /// <returns>JSON success response</returns>
        [HttpPost, ActionName("DecrementItemInShoppingCart")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DecrementItemInShoppingCart(int id)
        {
            // look up product entity
            Product product = await db.Products.SingleOrDefaultAsync(m => m.ProductId == id);
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
                return this.JError(403, e.Message);
            }

        }

        /// <summary>
        /// Removes a Product from the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to remove</param>
        /// <returns>JSON success response</returns>
        [HttpPost, ActionName("RemoveItemInShoppingCart")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveItemInShoppingCart(int id)
        {
            var x = db.Products;
            // look up product entity
            Product product = await db.Products.SingleOrDefaultAsync(m => m.ProductId == id);
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
                return this.JError(403, e.Message);
            }

        }

    }
}