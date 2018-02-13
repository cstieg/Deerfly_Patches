using Cstieg.Sales.Models;
using DeerflyPatches.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Orders
    /// </summary>
    [RoutePrefix("edit/orders")]
    [Route("{action}/{id?}")]
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Where(o => o.CustomerId != null)
                .Include(o => o.Customer).Include(o => o.ShipToAddress)
                .OrderByDescending(o => o.DateOrdered).Take(100);
            var orderList = await orders.ToListAsync();
            foreach (var order in orderList)
            {
                order.OrderDetails = await db.OrderDetails.Where(o => o.OrderId == order.Id).Include(o => o.Product).ToListAsync();
            }
            return View(orderList);
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Order order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
           
            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Product = await db.Products.FirstOrDefaultAsync(o => o.Id == orderDetail.ProductId);
            }
            return View(order);
        }
        
        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Product = await db.Products.FirstOrDefaultAsync(o => o.Id == orderDetail.ProductId);
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
