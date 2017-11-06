using System;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Deerfly_Patches.Modules.PayPal;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to handle PayPal interface
    /// </summary>
    public class PayPalController : Controller
    {
        private PayPalApiClient _paypalClient = new PayPalApiClient();

        public JsonResult GetOrderJson()
        {
            string country = Request.Params.Get("country");
            ShoppingCart shoppingCart;
            try
            {
                shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }

            shoppingCart.Country = country;

            if (country == "US")
            {
                shoppingCart.RemoveAllShippingCharges();
            }

            shoppingCart.SaveToSession(HttpContext);

            string orderJson;
            try
            {
                orderJson = _paypalClient.CreateOrder(shoppingCart);
            }
            catch (Exception e)
            {
                return this.JError(400, e.Message);
            }
            return Json(orderJson, JsonRequestBehavior.AllowGet);
        }
    }
}