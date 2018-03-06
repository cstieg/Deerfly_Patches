using Cstieg.FileHelper;
using Cstieg.Sales.PayPal;
using Cstieg.Sales.PayPal.Models;
using Cstieg.WebFiles;
using DeerflyPatches.Models;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Base controller to be provide basic behavior for all controllers
    /// </summary>
    public class BaseController : Controller
    {
        // PayPal service
        private string _paypalConfigFilePath = HostingEnvironment.MapPath("/paypal.json");
        private PayPalClientInfoService _payPalClientInfoService;
        private PayPalPaymentProviderService _payPalService;

        // storage service for storing uploaded images
        private const string _contentFolder = "/content";
        private const string _productImagesFolder = "images/products";
        private IFileService _storageService;
        protected ImageManager _productImageManager;

        protected ApplicationDbContext _context;

        public BaseController()
        {
            // Set storage service for product images
            _storageService = new FileSystemService(_contentFolder);
            _productImageManager = new ImageManager(_productImagesFolder, _storageService);
            _context = new ApplicationDbContext();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            filterContext.HttpContext.Response.AddCacheItemDependency("Pages");
        }

        protected async Task<PayPalClientInfoService> GetPayPalClientInfoServiceAsync()
        {
            if (_payPalClientInfoService != null)
                return _payPalClientInfoService;

            string clientInfoJson = await FileHelper.ReadAllTextAsync(_paypalConfigFilePath);
            _payPalClientInfoService = new PayPalClientInfoService(clientInfoJson);
            return _payPalClientInfoService;
        }

        protected async Task<PayPalClientAccount> GetActivePayPalClientAccountAsync()
        {
            return (await GetPayPalClientInfoServiceAsync()).GetClientAccount();
        }

        protected async Task<PayPalPaymentProviderService> GetPayPalServiceAsync()
        {
            if (_payPalService != null)
                return _payPalService;

            _payPalService = new PayPalPaymentProviderService(await GetPayPalClientInfoServiceAsync());
            return _payPalService;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}