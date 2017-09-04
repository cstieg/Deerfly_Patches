using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// A helper class containing code common to ModelControllers
    /// </summary>
    public class _ModelControllersHelper
    {
        private static string[] validImageTypes = new string[]
        {
            "image/gif",
            "image/jpeg",
            "image/png"
        };

        /// <summary>
        /// Validates and gets an image file from the POST request
        /// </summary>
        /// <param name="ModelState">The ModelState object from the controller</param>
        /// <param name="Request">The Request object from the current POST request</param>
        /// <param name="imageUrl">The URL of the image which may have been previously stored</param>
        /// <returns>The validated image file</returns>
        public static HttpPostedFileBase GetImageFile(ModelStateDictionary ModelState, HttpRequestBase Request, string imageUrl)
        {
            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = null;

            if (Request.Files.Count == 0)
            {
                // Don't add model error if there is already an image file
                if (!imageUrl.Equals(""))
                {
                    return null;
                }

                ModelState.AddModelError("ImageUrl", "This field is required");
            }
            else
            {
                imageFile = Request.Files[0];
            }

            if (imageFile != null && (imageFile.ContentLength == 0 || !validImageTypes.Contains(imageFile.ContentType)))
            {
                // Don't add model error if there is already an image file
                if (!imageUrl.Equals(""))
                {
                    return null;
                }

                ModelState.AddModelError("ImageUrl", "Please choose either a valid GIF, JPG or PNG image.");
            }

            return imageFile;
        }
    }
}