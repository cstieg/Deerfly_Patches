using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class ImageSaver : FileSaver
    {
        protected string[] _validImageTypes = new string[]
        {
            "image/gif",
            "image/jpeg",
            "image/png"
        };
        protected List<int> _imageSizes = new List<int>()
        {
            1600, 800, 400, 200, 100
        };

        public ImageSaver(string folder, string storageService = "", string[] validImageTypes = null, List<int> imageSizes = null) : base(folder, storageService)
        {
            if (validImageTypes != null)
            {
                _validImageTypes = validImageTypes;
            }
            if (imageSizes != null)
            {
                _imageSizes = imageSizes;
            }

            // sort descending
            _imageSizes.Sort(new Comparison<int>((i1, i2) => i2.CompareTo(i1)));
        }

        public new string SaveFile(HttpPostedFileBase file)
        {
            return SaveFile(file, null);
        }

        public string SaveFile(HttpPostedFileBase file, int? maxWidth)
        {
            if (_validImageTypes.Count() > 0 && !_validImageTypes.Contains(file.ContentType))
            {
                throw new InvalidFileTypeException();
            }

            return SaveFile(file.InputStream, file.FileName, maxWidth);
        }

        public string SaveFile(Stream stream, string name, int? maxWidth)
        {
            if (maxWidth != null)
            {
                ImageResizer imageResizer = new ImageResizer(stream);
                int imageWidth = imageResizer.GetImageWidth();
                if (imageWidth > maxWidth)
                {
                    Stream resizedStream = imageResizer.GetResizedImage((int)(maxWidth)).GetImageStream();
                    stream = resizedStream;
                    imageWidth = (int)(maxWidth);
                }
                name = GetResizedFileName(name, (int)(imageWidth));
            }

            return base.SaveFile(stream, name);
        }

        public string SaveImageMultipleSizes(HttpPostedFileBase imageFile, List<int> sizes = null)
        {
            return SaveImageMultipleSizes(imageFile.InputStream, imageFile.FileName, sizes);
        }

        public string SaveImageMultipleSizes(Stream stream, string name, List<int> sizes = null)
        {
            if (sizes == null)
            {
                sizes = _imageSizes;
            }

            List<string> srcSetItems = new List<string>();

            // For small images (smaller than largest desired width), remove target sizes that are larger than original image,
            // so as not to attempt expanding image.
            int imageWidth = new ImageResizer(stream).GetImageWidth();
            sizes = GetAdjustedSizeList(sizes, imageWidth);

            MemoryStream memoryStream = stream.CloneToMemoryStream();

            for (var i = 0; i < sizes.Count; i++)
            {
                string url = SaveFile(memoryStream, name, sizes[i]);
                srcSetItems.Add(url + " " + sizes[i] + "w");
            }

            string srcset = string.Join(", ", srcSetItems);
            return srcset;
        }

        public string GetResizedFileName(string filename, int size)
        {
            Filename imageFilename = new Filename(filename);
            return imageFilename.BaseName + "-w" + size.ToString() + imageFilename.Extension;
        }

        public List<int> GetAdjustedSizeList(List<int> sizes, int originalWidth)
        {
            sizes = sizes.Clone();
            Boolean isUndersizedImage = false;
            sizes.ForEach(s =>
            {
                if (s > originalWidth)
                {
                    sizes.Remove(s);
                    isUndersizedImage = true;
                }
            });
            // Add original (largest possible) size in case of small image
            if (isUndersizedImage)
            {
                sizes.Add(originalWidth);
            }
            return sizes;
        }
    }
}