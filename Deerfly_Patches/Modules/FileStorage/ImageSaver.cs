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
                if (imageResizer.GetImageWidth() > maxWidth)
                {
                    Stream resizedStream = imageResizer.GetResizedImage((int)(maxWidth)).GetImageStream();
                    stream = resizedStream;
                }
            }

            return base.SaveFile(stream, name);
        }

        public string SaveImageMultipleSizes(Stream stream, string name, List<int> sizes = null)
        {
            if (sizes == null)
            {
                sizes = _imageSizes;
            }

            List<string> srcSetItems = new List<string>();

            MemoryStream memoryStream = stream.CloneToMemoryStream();

            for (var i = 0; i < sizes.Count; i++)
            {
                // Todo: check if image sizes is larger than desired sizes; if not, return largest possible, skip others
                Filename imageFilename = new Filename(name);
                string url = SaveFile(memoryStream, imageFilename.BaseName + "-w" + sizes[i].ToString() + imageFilename.Extension, sizes[i]);
                srcSetItems.Add(url + " " + sizes[i] + "w");
            }

            string srcset = string.Join(", ", srcSetItems);
            return srcset;


        }

        public string SaveImageMultipleSizes(HttpPostedFileBase imageFile, List<int> sizes = null)
        {
            return SaveImageMultipleSizes(imageFile.InputStream, imageFile.FileName, sizes);
        }

    }
}