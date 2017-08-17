using System.Web.Mvc;

using Deerfly_Patches.Models;
using Deerfly_Patches.Modules;
using Deerfly_Patches.ViewModels;

namespace Deerfly_Patches.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Checkout
        public ActionResult Index()
        {
            ViewBag.session = Session.Contents;
            return View();
        }

        [HttpPost, ActionName("UpdateShippingAddress")]
        public ActionResult UpdateShippingAddress()
        {

            Address shippingAddress = new Address()
            {
                Recipient = Request.Form["ship-to"],
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

            // TODO: Save address to model


            return this.JOk();
        }

    }
}