using System.Web.Mvc;
using Microsoft.Owin;
using Deerfly_Patches.ViewModels;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        public ActionResult Index()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            return View(shoppingCart);
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