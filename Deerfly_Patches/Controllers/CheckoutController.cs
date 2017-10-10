using System.Web.Mvc;

using Deerfly_Patches.Models;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.PayPal;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller to handle checkout of shopping cart through PayPal
    /// </summary>
    public class CheckoutController : Controller
    {
        private PayPalApiClient _paypalClient = new PayPalApiClient();

        // GET: Checkout
        public ActionResult Index()
        {
            ViewBag.ClientInfo = _paypalClient.GetClientSecrets();
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            return View(shoppingCart);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("UpdateShippingAddress")]
        public ActionResult UpdateShippingAddress()
        {
            Address shippingAddress = new Address()
            {
                Recipient = Request.Form["recipient"],
                Address1 = Request.Form["address1"],
                Address2 = Request.Form["address2"],
                City = Request.Form["city"],
                State = Request.Form["state"],
                Zip = Request.Form["zip"],
                Country = Request.Form["country"],
                Phone = Request.Form["phone"]
            };

            // Get shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            shoppingCart.GetOrder().ShipToAddress = shippingAddress;
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);

            return View("Index", shoppingCart);
        }

    }
}