using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Deerfly_Patches.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Orders
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Include(o => o.Customer).OrderByDescending(o => o.DateOrdered);
            var orderList = await orders.ToListAsync();
            foreach (var order in orderList)
            {
                order.OrderDetails = await db.OrderDetails.Where(o => o.OrderId == order.OrderId).Include(o => o.Product).ToListAsync();
            }
            return View(orderList);
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
           
            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Product = await db.Products.FindAsync(orderDetail.ProductId);
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
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.Product = await db.Products.FindAsync(orderDetail.ProductId);
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
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
