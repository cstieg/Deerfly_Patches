using System.Web.Mvc;
using Deerfly_Patches.Modules;
using Deerfly_Patches.Modules.PayPal;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        public ActionResult Index()
        {
            ClientInfo clientInfo = new PayPalApiClient().GetClientSecrets();
            ViewBag.ClientInfo = clientInfo;
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            return View(shoppingCart);
        }
    }
}