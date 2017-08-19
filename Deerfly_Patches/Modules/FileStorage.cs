using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules
{
    public class FileStorage
    {
        public string SaveImage(HttpPostedFileBase imageFile)
        {
            // Save image to disk and store filepath in model
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