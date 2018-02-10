using Cstieg.ControllerHelper;
using Cstieg.Sales.Models;
using DeerflyPatches.Models;
using RazorEngine;
using RazorEngine.Templating;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    public class MailController : BaseController
    {
        // POST: Mail/ConfirmOrder?cart=DF39FEI314040
        /// <summary>
        /// Sends confirmation email for completed order.
        /// </summary>
        /// <param name="cart">Alphanumeric cart id assigned to order by PayPal</param>
        /// <returns>JSON success=true result </returns>
        [HttpPost]
        public async Task<ActionResult> ConfirmOrder()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            // find order
            string id = Request.Params.Get("cart");
            Order order = await db.Orders.Include(o => o.Customer).Where(o => o.Cart == id).SingleOrDefaultAsync();
            if (order == null)
            {
                return HttpNotFound();
            }
            order.ShipToAddress = await db.Addresses.FirstOrDefaultAsync(o => o.Id == order.ShipToAddressId);

            // Add baseURL for images to viewBag
            var viewBag = new DynamicViewBag();
            string baseUrl = Server.MapPath("~");
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
            viewBag.AddValue("baseURL", baseUrl);

            // render email body
            string templatePath = Server.MapPath("~/Views/Mail/OrderSuccessEmail.cshtml");
            var sr = new StreamReader(templatePath);
            string body = Engine.Razor.RunCompile(await sr.ReadToEndAsync(), "orderSuccessEmail", null, order, viewBag);

            // create email message
            var message = new MailMessage
            {
                From = new MailAddress("webmaster@stovepipeheatsaver.com"),
                Subject = "Order confirmation - " + order.Description,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(order.Customer.EmailAddress));

            // send email
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
            return this.JOk();
        }
    }
}