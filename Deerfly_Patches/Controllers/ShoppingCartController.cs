using System.Web.Mvc;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.PayPal;
using Deerfly_Patches.Models;
using System.Linq;
using System;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            ViewBag.ClientInfo = new PayPalApiClient().GetClientSecrets();
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            return View(shoppingCart);
        }

        [HttpPost]
        public ActionResult AddPromoCode()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            try
            {
                string pc = Request.Params.Get("PromoCode");
                PromoCode promoCode = db.PromoCodes.Where(p => p.Code.ToLower() == pc.ToLower()).Single();

                shoppingCart.AddPromoCode(promoCode);
                HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);

                return Redirect("Index");
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: Invalid promo code");
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