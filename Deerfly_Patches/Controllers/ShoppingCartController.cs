using Cstieg.ControllerHelper;
using Cstieg.FileHelper;
using Cstieg.Sales;
using Cstieg.Sales.Exceptions;
using Cstieg.Sales.Models;
using Cstieg.Sales.PayPal;
using Cstieg.Sales.PayPal.Models;
using DeerflyPatches.Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : BaseController
    {
        private string _paypalConfigFilePath = HostingEnvironment.MapPath("/paypal.json");

        private ApplicationDbContext db = new ApplicationDbContext();
        private ShoppingCartService _shoppingCartService;

        /// <summary>
        /// Initialize settings that are unabled to be initialized in constructor
        /// </summary>
        protected async override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            _shoppingCartService = new ShoppingCartService(db, Request.AnonymousID);
        }

        // GET: ShoppingCart
        public async Task<ActionResult> Index()
        {
            ViewBag.ClientInfo = await GetPayPalClientAccountAsync();
            //ViewBag.Countries = await db.Countries.ToListAsync();
            ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartAsync();
            return View(shoppingCart);
        }

        // GET: ShoppingCart/OrderSuccess?cart=DF39FEI314040
        /// <summary>
        /// Displays confirmation for completed order
        /// </summary>
        /// <param name="cart">Alphanumeric cart id assigned to order by PayPal</param>
        public async Task<ActionResult> OrderSuccess(string cart)
        {
            Order order = await db.Orders.Include(o => o.Customer).SingleOrDefaultAsync(o => o.Cart == cart);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.ShipToAddress = await db.Addresses.FirstAsync(a => a.Id == order.ShipToAddressId);
            return View(order);
        }

        /// <summary>
        /// Gets the number of items in the shopping cart
        /// </summary>
        /// <returns>A JSON object containing the number of items in the shopping cart in the field shoppingCartCount</returns>
        public async Task<JsonResult> ShoppingCartCount()
        {
            ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartAsync();
            return Json(new { shoppingCartCount = shoppingCart.GetOrderDetails().Count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response if successful, error response if product already exists</returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int id)
        {
            try
            {
                var orderDetail = await _shoppingCartService.AddProductAsync(id);
            }
            catch (ProductAlreadyInShoppingCartException) { }
            catch (NotFoundException e)
            {
                return HttpNotFound(e.Message);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
            return this.JOk();
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
            try
            {
                await _shoppingCartService.RemoveProductAsync(id);
                var shoppingCart = await _shoppingCartService.GetShoppingCartAsync();
                return this.JOk(new { needsRefresh = shoppingCart.NeedsRefresh });
            }
            catch (NotFoundException e)
            {
                return HttpNotFound(e.Message);
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
            try
            {
                var orderDetail = await _shoppingCartService.IncrementProductAsync(id);
                return this.JOk(new { orderDetail = orderDetail });
            }
            catch (NotFoundException e)
            {
                return HttpNotFound(e.Message);
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
            try
            {
                var orderDetail = await _shoppingCartService.DecrementProductAsync(id);
                return this.JOk(new { orderDetail = orderDetail } );
            }
            catch (NotFoundException e)
            {
                return HttpNotFound(e.Message);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        /// <summary>
        /// Sets the country and updates shipping charges accordingly
        /// </summary>
        /// <param name="country">The 2 digit country code where the order will be shipped</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetCountry(string country)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.SetCountryAsync(country);
                return this.JOk(shoppingCart);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddPromoCode(string promoCode)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.AddPromoCodeAsync(promoCode);
                return Redirect("Index");
            }
            catch (InvalidPromoCodeException e)
            {
                ModelState.AddModelError("PromoCodesAdded", "Failed to add promocode: Invalid promo code - " + e.Message);
                ViewBag.ClientInfo = await GetPayPalClientAccountAsync();
                return View("Index", await _shoppingCartService.GetShoppingCartAsync());
            }
            catch (Exception e)
            {
                ModelState.AddModelError("PromoCodesAdded", "Failed to add promocode: " + e.Message);
                ViewBag.ClientInfo = await GetPayPalClientAccountAsync();
                return View("Index", await _shoppingCartService.GetShoppingCartAsync());
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemovePromoCode(string promoCode)
        {
            try
            {
                var shoppingCart = await _shoppingCartService.RemovePromoCodeAsync(promoCode);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

    }
}