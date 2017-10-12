using System.Web.Mvc;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.PayPal;
using Deerfly_Patches.Models;
using System.Linq;

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
            string pc = Request.Params.Get("PromoCode");
            PromoCode promoCode = db.PromoCodes.Where(p => p.Code.ToLower() == pc.ToLower()).Single();

            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            shoppingCart.AddPromoCode(promoCode);
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);

            return Redirect("Index");
        }

        [HttpPost]
        public ActionResult RemovePromoCode()
        {

            return Index();
        }
    }
}