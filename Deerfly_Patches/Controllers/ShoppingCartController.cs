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


        public ActionResult OrderSuccess()
        {
            // save shopping cart
            // do we need to set modified state for each nested level?
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            db.Orders.Add(shoppingCart.Order);
            
            for (int i = 0; i < shoppingCart.Order.OrderDetails.Count; i++)
            {
                db.OrderDetails.Add(shoppingCart.Order.OrderDetails[i]);
            }
            db.SaveChanges();


            return View();
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