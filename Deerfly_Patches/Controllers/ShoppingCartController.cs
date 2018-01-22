using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Cstieg.Sales.Models;
using Cstieg.Sales.PayPal;
using DeerflyPatches.Models;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : BaseController
    {
       ClientInfo ClientInfo = new PayPalApiClient().GetClientSecrets();

        // GET: ShoppingCart
        public async Task<ActionResult> Index()
        {
            var db = new ApplicationDbContext();
            ViewBag.ClientInfo = ClientInfo;
            //ViewBag.Countries = await db.Countries.ToListAsync();
            ShoppingCart shoppingCart = await GetShoppingCart(db);
            return View(shoppingCart);
        }

        // GET: ShoppingCart/OrderSuccess?cart=DF39FEI314040
        /// <summary>
        /// Displays confirmation for completed order
        /// </summary>
        /// <param name="cart">Alphanumeric cart id assigned to order by PayPal</param>
        public async Task<ActionResult> OrderSuccess()
        {
            var db = new ApplicationDbContext();
            string id = Request.Params.Get("cart");
            Order order = await db.Orders.Include(o => o.Customer).Where(o => o.Cart == id).SingleOrDefaultAsync();
            if (order == null)
            {
                return HttpNotFound();
            }
            int addressId = (int) order.ShipToAddressId;
            ShipToAddress address = await db.Addresses.FindAsync(addressId);
            order.ShipToAddress = address;
            return View(order);
        }
        
        /// <summary>
        /// Gets the number of items in the shopping cart
        /// </summary>
        /// <returns>A JSON object containing the number of items in the shopping cart in the field shoppingCartCount</returns>
        public async Task<JsonResult> ShoppingCartCount()
        {
            var db = new ApplicationDbContext();
            ShoppingCart shoppingCart = await GetShoppingCart(db);
            return Json(new { shoppingCartCount = shoppingCart.Order.OrderDetails.Count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response if successful, error response if product already exists</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int id)
        {
            var db = new ApplicationDbContext();
            // look up product entity
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            try
            {
                ShoppingCart shoppingCart = await GetShoppingCart(db);
                shoppingCart.AddProduct(product);
                await SaveShoppingCart(shoppingCart, db);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        /// <summary>
        /// Increases the quantity of a product in the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IncrementItem(int id)
        {
            var db = new ApplicationDbContext();
            // look up product entity
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Increment quantity and save shopping cart
            try
            {
                ShoppingCart shoppingCart = await GetShoppingCart(db);
                var orderDetail = shoppingCart.IncrementProduct(product);
                await SaveShoppingCart(shoppingCart, db);
                return this.JOk(orderDetail);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        /// <summary>
        /// Decreases the quantity of an item in the shopping cart
        /// </summary>
        /// <param name="id">ID of the Product model qty to decrement</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DecrementItem(int id)
        {
            var db = new ApplicationDbContext();
            // look up product entity
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Decrement qty and update shopping cart
            try
            {
                ShoppingCart shoppingCart = await GetShoppingCart(db);
                var orderDetail = shoppingCart.DecrementProduct(product);
                await SaveShoppingCart(shoppingCart, db);
                return this.JOk(orderDetail);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        /// <summary>
        /// Removes a Product from the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to remove</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveItem(int id)
        {
            var db = new ApplicationDbContext();
            try
            {
                ShoppingCart shoppingCart = await GetShoppingCart(db);
                OrderDetail orderDetail = shoppingCart.Order.OrderDetails.Find(o => o.ProductId == id);
                if (orderDetail == null)
                {
                    return HttpNotFound();
                }

                db.OrderDetails.Remove(orderDetail);
                await db.SaveChangesAsync();

                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(403, e.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetCountry()
        {
            var db = new ApplicationDbContext();
            string country = Request.Params.Get("country");
            ShoppingCart shoppingCart = await GetShoppingCart(db);

            shoppingCart.Country = country;
            //shoppingCart.UpdateShippingCharges();
            // ************************************* Deerflypatches.com specific ***********************************
            if (country == "US")
            {
                shoppingCart.RemoveAllShippingCharges();
            }
            else
            {
                shoppingCart.ResetShippingCharges();
            }


            try
            {
                await SaveShoppingCart(shoppingCart, db);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
            return this.JOk(shoppingCart);
        }

        [HttpPost]
        public async Task<ActionResult> AddPromoCode()
        {
            var db = new ApplicationDbContext();
            ShoppingCart shoppingCart = await GetShoppingCart(db);
            try
            {
                string pc = Request.Params.Get("PromoCode");
                PromoCode promoCode = await db.PromoCodes.Where(p => p.Code.ToLower() == pc.ToLower()).SingleAsync();
                
                shoppingCart.AddPromoCode(promoCode);

                await SaveShoppingCart(shoppingCart, db);
                return Redirect("Index");
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: Invalid promo code - " + e.Message);
                ViewBag.ClientInfo = new PayPalApiClient().GetClientSecrets();
                return View("Index", shoppingCart);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: " + e.Message);
                ViewBag.ClientInfo = new PayPalApiClient().GetClientSecrets();
                return View("Index", shoppingCart);
            }
        }

    }
}