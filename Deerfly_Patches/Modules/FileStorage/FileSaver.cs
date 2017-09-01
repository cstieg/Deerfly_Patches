using System;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileSaver : IFileSaver
    {
        private IFileSaver _fileSaver;
        private string _storageService;
        private string _folder;
        
        public FileSaver(string storageService = "", string folder = "")
        {
            if (storageService == "")
            {
                _storageService = RouteConfig.storageService;
            }
            else
            {
                _storageService = storageService;
            }

            _folder = folder;

            switch (_storageService)
            { 
                case "AzureBlob":
                    if (folder == "")
                    {
                        throw new Exception("Container is required for Azure Blob Service");
                    }
                    _fileSaver = new AzureBlobService("AzureStorageConnectionString", folder);
                    break;

                case "fileSystem":
                    if (folder != "")
                    {
                        folder = "/" + folder;
                    }
                    _fileSaver = new FileSystemService(RouteConfig.contentFolder + folder);
                    break;
            }
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength != 0)
            {
                return _fileSaver.SaveFile(file);
            }
            return "";
            
        }
    }
}