using Cstieg.FileHelper;
using Cstieg.Sales.PayPal;
using Cstieg.Sales.PayPal.Models;
using Cstieg.WebFiles;
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
        private string _paypalConfigFilePath = HostingEnvironment.MapPath("/paypal.json");
        private PayPalClientInfoService _payPalClientInfoService;
        private PayPalPaymentProviderService _payPalService;

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

        protected async Task<PayPalClientInfoService> GetPayPalClientInfoServiceAsync()
        {
            if (_payPalClientInfoService != null) return _payPalClientInfoService;

            string clientInfoJson = await FileHelper.ReadAllTextAsync(_paypalConfigFilePath);
            _payPalClientInfoService = new PayPalClientInfoService(clientInfoJson);
            return _payPalClientInfoService;
        }

        protected async Task<PayPalClientAccount> GetPayPalClientAccountAsync()
        {
            return (await GetPayPalClientInfoServiceAsync()).GetClientAccount();
        }

        protected async Task<PayPalPaymentProviderService> GetPayPalService()
        {
            if (_payPalService != null) return _payPalService;

            _payPalService = new PayPalPaymentProviderService(await GetPayPalClientInfoServiceAsync());
            return _payPalService;
        }
    }
}