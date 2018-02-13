using Cstieg.Sales.RSS;
using DeerflyPatches.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller for RSS feeds
    /// </summary>
    public class RssController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private GoogleItemRssService _rssService;

        public RssController()
        {
            _rssService = new GoogleItemRssService(_context);
        }

        // GET: /Rss/Products
        /// <summary>
        /// Gets an RSS file containing product information to be consumed by Google Shopping
        /// </summary>
        /// <returns>A text file containing all of the product information in RSS XML format</returns>
        public async Task<FileResult> Products()
        {
            var rssStream = await _rssService.GetRssAsync();

            return new FileStreamResult(rssStream, "application/rss+xml")
            {
                 FileDownloadName = "products.txt"
            };
        }
    }
}