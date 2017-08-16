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

            Address shippingAddress = new Address();
            shippingAddress.Recipient = Request.Form["ship-to"];
            shippingAddress.Address1 = Request.Form["address1"];
            shippingAddress.Address2 = Request.Form["address2"];
            shippingAddress.City = Request.Form["city"];
            shippingAddress.State = Request.Form["state"];
            shippingAddress.Zip = Request.Form["zip"];
            shippingAddress.Country = Request.Form["country"];
            shippingAddress.Phone = Request.Form["phone"];

            // Get shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            shoppingCart.GetOrder().ShipToAddress = shippingAddress;
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);

            // TODO: Save address to model


            return this.JOk();
        }

    }
}