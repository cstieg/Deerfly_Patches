using System.Threading.Tasks;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.PayPal;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to handle PayPal interface
    /// </summary>
    public class PayPalController : Controller
    {
        private PayPalApiClient _paypalClient = new PayPalApiClient();

        public string GetOrderJson()
        {
            string country = Request.Params.Get("country");

            // get shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            if (country == "US")
            {
                shoppingCart.RemoveAllShippingCharges();
            }

            return _paypalClient.CreateOrder(shoppingCart);
        }


        /// <summary>
        /// Gets the user's profile information from PayPal
        /// </summary>
        /// <returns>View that closes the login window and redirects to checkout page</returns>
        public async Task<ActionResult> GetUserInfo()
        {
            // authorization code passed in from login to paypal on frontend
            string authorizationCode = Request.Params.Get("code");
            Session.Add("PayPalAuthorizationCode", authorizationCode);

            // make API call to PayPal using authorization code to get user's information
            UserInfo userInfo = await _paypalClient.GetUserInfo(authorizationCode);

            // get shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");

            // copy user info to shipping address in shopping cart
            if (shoppingCart.Order.ShipToAddress == null)
            {
                shoppingCart.Order.ShipToAddress = new Address();
            }
            Address shippingAddress = shoppingCart.Order.ShipToAddress;
            shippingAddress.Recipient = userInfo.Name;
            shippingAddress.Address1 = userInfo.Address.StreetAddress;
            shippingAddress.City = userInfo.Address.City;
            shippingAddress.State = userInfo.Address.State;
            shippingAddress.Zip = userInfo.Address.PostalCode;
            shippingAddress.Country = userInfo.Address.Country;
            shippingAddress.Type = AddressType.Shipping;
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);

            // close login window and redirect to checkout page
            return View("CloseLoginWindow");
        }

        /// <summary>
        /// Creates the order to be posted to the PayPal API
        /// </summary>
        /// <returns>A JSON string containing the order information</returns>
        [HttpPost]
        public async Task<string> CreateOrder()
        {
            // Get shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            if (shoppingCart.Order.ShipToAddress == null)
            {
                shoppingCart.Order.ShipToAddress = new Address();
            }

            // TODO: Need to replace this -- need country list
            shoppingCart.Order.ShipToAddress.Country = "US";

            // Create JSON string with order information
            shoppingCart.PayeeEmail = _paypalClient.ClientInfo.ClientAccount;

            // Post order to PayPal API and return order ID to front end
            return await _paypalClient.PostOrder(shoppingCart);
        }

        /// <summary>
        /// Posts the order to PayPal Payments API
        /// </summary>
        /// <param name="paymentId">Payment ID created by PayPal frontend order confirmation endpoint</param>
        /// <param name="data">Data passed by PayPal frontend order confirmation endpoint</param>
        /// <returns>Json success string</returns>
        [HttpPost]
        public async Task<ActionResult> ExecutePayment()
        {
            string payerId = Request.Form["PayerID"];
            string paymentId = Request.Form["PaymentID"];
            await _paypalClient.ExecutePayment(paymentId, payerId);
            return this.JOk();
        }
    }
}