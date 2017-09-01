using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class ImageSaver : FileSaver
    {
        private string[] _validImageTypes;

        public ImageSaver(string[] validImageTypes = null)
        {
            if (validImageTypes == null)
            {
                _validImageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/png"
                };
            }
            else
            {
                _validImageTypes = validImageTypes;
            }
        }


        public new string SaveFile(HttpPostedFileBase imageFile, int? maxWidth)
        {
            if (_validImageTypes.Count() > 0 && !_validImageTypes.Contains(imageFile.ContentType))
            {
                throw new InvalidFileTypeException();
            }
            
            if (maxWidth != null)
            {
                Stream resizedStream = new ImageResizer(imageFile.InputStream).GetResizedImage((int)(maxWidth)).GetImageStream();
                
            }

            return base.SaveFile(imageFile);
        }

        public string SaveImageMultipleSizes(HttpPostedFileBase imageFile, List<int> sizes = null)
        {
            string baseUrl = SaveFile(imageFile);




            return "";
        }

    }
}