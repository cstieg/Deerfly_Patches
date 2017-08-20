using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Mvc;

namespace Deerfly_Patches.Controllers
{
    public class BlobController : Controller
    {
        public ActionResult CreateBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("dfp_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("product-images");
            ViewBag.Success = blobContainer.CreateIfNotExists();
            ViewBag.BlobContainerName = blobContainer.Name;
            return View();
        }

        public EmptyResult UploadBlob()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("dfp_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("product-images");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference("product-image");
            return new EmptyResult();
        }
    }
}