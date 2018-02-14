using Cstieg.ControllerHelper;
using Cstieg.Sales;
using Cstieg.Sales.Models;
using Cstieg.Sales.Repositories;
using DeerflyPatches.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class PayPalController : BaseController
    {
        private ISalesDbContext _context = new ApplicationDbContext();
        private IShoppingCartService _shoppingCartService;
    
        // Initialize variables needing requestContext, unable to initialize in controller
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            // Get anonymous ID for user from cookie to identify shopping cart that may have been saved previously
            _shoppingCartService = new ShoppingCartService(_context, Request.AnonymousID);
        }

        /// <summary>
        /// Gets a PayPal object representation of the order in the shopping cart
        /// </summary>
        /// <param name="country">2-digit country code of the country selected in the shopping cart</param>
        /// <returns>A JSON object in PayPal order format describing payment details</returns>
        public async Task<JsonResult> GetOrderJson(string country)
        {
            try
            {
                ShoppingCart shoppingCart = await _shoppingCartService.SetCountryAsync(country);
                string orderJson = (await GetPayPalService()).CreatePaymentDetails(shoppingCart);
                return Json(orderJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }

        // POST /PayPal/VerifyAndSave?paymentDetails={.....}
        /// <summary>
        /// Verifies and saves the shopping cart
        /// </summary>
        /// <param name="paymentDetails">The payment details object created by PayPal create order API call</param>
        /// <returns>Json success</returns>
        [HttpPost]
        public async Task<JsonResult> VerifyAndSave(string paymentDetails)
        {
            try
            {
                var _payPalService = await GetPayPalService();
                _payPalService.SetPaymentResponse(paymentDetails);
                var shipToAddress = _payPalService.GetShippingAddress();
                var customer = _payPalService.GetCustomer();
                var cartId = _payPalService.GetCartId();
                var order = await _shoppingCartService.CheckoutAsync(shipToAddress, shipToAddress, customer, cartId);

                // On success, front end will execute payment with PayPal
                return this.JOk(order);
            }
            
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
        }
        
    }
}