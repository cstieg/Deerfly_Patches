using Cstieg.ControllerHelper;
using Cstieg.Sales.Models;
using DeerflyPatches.Models;
using RazorEngine;
using RazorEngine.Templating;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    public class MailController : BaseController
    {
        public async Task Test()
        {
            string cart = "PAY-8279066855591091LLJ7BRAA";
            await ConfirmOrder(cart);
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
            ApplicationDbContext db = new ApplicationDbContext();

            // find order
            Order order = await db.Orders.Include(o => o.Customer).Where(o => o.Cart == cart).SingleOrDefaultAsync();
            if (order == null)
            {
                return HttpNotFound();
            }
            var a = order.OrderDetails.First().Product.WebImages.First().ImageUrl;
            order.ShipToAddress = await db.Addresses.FirstOrDefaultAsync(o => o.Id == order.ShipToAddressId);
            
            // Add baseURL for images to viewBag
            var viewBag = new DynamicViewBag();
            string baseUrl = ControllerHelper.GetBaseUrl(Request);
            viewBag.AddValue("baseURL", baseUrl);

            // render email body
            string templatePath = Server.MapPath("~/Views/Mail/OrderSuccessEmail.cshtml");
            var sr = new StreamReader(templatePath);
            string body = Engine.Razor.RunCompile(await sr.ReadToEndAsync(), "OrderSuccessEmail", null, order, viewBag);

            // create email message
            var message = new MailMessage
            {
                Subject = "Order confirmation - " + order.Description,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(order.Customer.EmailAddress));

            // send email
            using (var smtp = new SmtpClient())
            {
                // get sender account from web.config - system.net
                string from = ((NetworkCredential)smtp.Credentials).UserName;
                message.From = new MailAddress(from);
               
                await smtp.SendMailAsync(message);
            }
            return this.JOk();
        }
    }
}