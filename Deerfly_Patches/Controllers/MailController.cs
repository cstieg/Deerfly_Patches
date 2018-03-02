using Cstieg.ControllerHelper;
using Cstieg.Sales;
using Cstieg.Sales.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    public class MailController : BaseController
    {
        private ShoppingCartService _shoppingCartService;

        public async Task<ActionResult> Test()
        {
            string cart = "PAY-TEST";
            return await ConfirmOrder(cart);
        }

        // POST: Mail/ConfirmOrder?cart=DF39FEI314040
        /// <summary>
        /// Sends confirmation email for completed order.
        /// </summary>
        /// <param name="cart">Alphanumeric cart id assigned to order by PayPal</param>
        /// <returns>JSON success=true result </returns>
        [HttpPost]
        public async Task<ActionResult> ConfirmOrder(string cart)
        {
            // find order
            Order order = await _context.Orders.Include(o => o.Customer).Include(o => o.ShipToAddress)
                .SingleOrDefaultAsync(o => o.Cart == cart);
            if (order == null)
            {
                return HttpNotFound();
            }
            _shoppingCartService = new ShoppingCartService(_context, Request.AnonymousID);

            string templatePath = Server.MapPath("~/Views/Mail/OrderSuccessEmail.cshtml");
            await _shoppingCartService.SendConfirmationEmailAsync(order, ControllerHelper.GetBaseUrl(Request), templatePath);

            return this.JOk();
        }
    }
}