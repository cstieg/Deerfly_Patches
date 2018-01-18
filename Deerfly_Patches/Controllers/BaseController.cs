using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cstieg.Sales.Models;
using Cstieg.WebFiles;
using DeerflyPatches.Models;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Base controller to be provide basic behavior for all controllers
    /// </summary>
    public class BaseController : Controller
    {
        public static string contentFolder = "/content";

        // storage service for storing uploaded images
        protected IFileService storageService;
        protected ImageManager imageManager;

        public BaseController()
        {
            // Set storage service for product images
            storageService = new FileSystemService(contentFolder);
            imageManager = new ImageManager("images/products", storageService);
        }

        /// <summary>
        /// Add dependency to cache so it is refreshed when updating the dependency
        /// </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            filterContext.HttpContext.Response.AddCacheItemDependency("Pages");
        }

        /// <summary>
        /// Saves the shopping cart object for the current anonymous user in the database, using the AnonymousId from cookie to identify the owner.
        /// </summary>
        /// <param name="shoppingCart">ShoppingCart object to save</param>
        /// <param name="db">Database context to which ShoppingCart belongs</param>
        protected async Task SaveShoppingCart(ShoppingCart shoppingCart, ApplicationDbContext db)
        {

            shoppingCart.OwnerId = Request.AnonymousID;
            if (await db.ShoppingCarts.AnyAsync(s => s.OwnerId == Request.AnonymousID))
            {
                db.Entry(shoppingCart).State = EntityState.Modified;
                db.Entry(shoppingCart.Order).State = EntityState.Modified;
                foreach (var orderDetail in shoppingCart.Order.OrderDetails)
                {
                    db.Entry(orderDetail.Product).State = EntityState.Unchanged;
                    db.Entry(orderDetail.Order).State = EntityState.Unchanged;
                }
            }
            else
            {
                db.ShoppingCarts.Add(shoppingCart);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the shopping cart object for the current anonymous user (according to AnonymousId stored in cookie) from database.
        /// </summary>
        /// <returns>The shopping cart object for the current user</returns>
        /// <param name="db">Database context to which ShoppingCart belongs</param>
        protected async Task<ShoppingCart> GetShoppingCart(ApplicationDbContext db)
        {
            return await db.ShoppingCarts.Include(s => s.Order)
                                         .Include(s => s.Order.OrderDetails)
                                         .Where(s => s.OwnerId == Request.AnonymousID)
                                         .SingleOrDefaultAsync() ?? new ShoppingCart();
        }

        /// <summary>
        /// Deletes a shopping cart object from the database
        /// </summary>
        /// <param name="shoppingCart">The shopping cart object to delete</param>
        /// <param name="db">Database context to which ShoppingCart belongs</param>
        protected async Task DeleteShoppingCart(ShoppingCart shoppingCart, ApplicationDbContext db)
        {
            db.Entry(shoppingCart).State = EntityState.Deleted;
            await db.SaveChangesAsync();
        }
    }
}