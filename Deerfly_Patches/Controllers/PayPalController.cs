using System.Web.Mvc;
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
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            if (country == "US")
            {
                shoppingCart.Order.ShipToAddress.Country = country;
                shoppingCart.RemoveAllShippingCharges();
            }

            shoppingCart.SaveToSession(HttpContext);
            return _paypalClient.CreateOrder(shoppingCart);
        }
    }
}