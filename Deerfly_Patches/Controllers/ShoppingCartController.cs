using System.Web.Mvc;

using Deerfly_Patches.ViewModels;
using Deerfly_Patches.Modules;

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
    }
}