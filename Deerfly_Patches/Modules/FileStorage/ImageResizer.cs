using System.IO;
using System.Web.Helpers;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class ImageResizer
    {
        WebImage _webImage;


        public ImageResizer(Stream imageStream )
        {
            _webImage = new WebImage(imageStream);
        }

        public WebImage GetResizedImage(int width)
        {
            int originalWidth = _webImage.Width;
            int originalHeight = _webImage.Height;
            float aspectRatio = (float)(originalWidth) / originalHeight;
            int height = (int)(originalWidth / aspectRatio);
            WebImage resizedImage = _webImage.Resize(width, height, true);
            return resizedImage;
        }

        public void SaveAs(string filePath)
        {
            _webImage.FileName = filePath;
            _webImage.Save();
        }

        public void SaveResizedImage(string filePath, int width)
        {
            GetResizedImage(width);
            SaveAs(filePath);
        }

        public void SaveImageAsSizes(string filePath, int[] widths, int defaultWidth = 480)
        {

        }


    }

    public static class WebImageHelper
    {
        public static Stream GetImageStream(this WebImage webImage)
        {
            return new MemoryStream(webImage.GetBytes());
        }
    }
}