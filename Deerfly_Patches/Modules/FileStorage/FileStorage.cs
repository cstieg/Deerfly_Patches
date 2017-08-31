using Deerfly_Patches.Modules.Azure;
using System.IO;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileStorage
    {
        private AzureBlobService _blobService;

        public FileStorage()
        {
            _blobService = new AzureBlobService("dfp_AzureStorageConnectionString", "product-images");
        }

        public string SaveImage(HttpPostedFileBase imageFile)
        {
            // Save image to Azure Blob
            if (RouteConfig.storeInCloud)
            {
                return _blobService.UploadFile(imageFile);
            }

            // Save image to disk
            if (imageFile != null && imageFile.ContentLength != 0)
            {
                var imagePath = Path.Combine(RouteConfig.productImagesPath, imageFile.FileName);
                var imageUrl = RouteConfig.productImagesFolder + "/" + imageFile.FileName;
                imageFile.SaveAs(imagePath);
                return imageUrl;
            }
            return "";
        }
    }
}