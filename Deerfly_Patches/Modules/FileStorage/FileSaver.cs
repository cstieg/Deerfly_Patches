using System;
using System.IO;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileSaver : IFileSaver
    {
        protected IFileSaver _fileSaver;
        protected string _storageService;
        protected string _folder;
        
        public FileSaver(string folder, string storageService = "")
        {
            _folder = folder;

            if (storageService == "")
            {
                _storageService = RouteConfig.storageService;
            }
            else
            {
                _storageService = storageService;
            }

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

        public string SaveFile(Stream stream, string name)
        {
            if (stream.Length == 0)
            {
                throw new NoDataException("There is no data in this stream!");
            }

            name = name.Replace(' ', '_');
            return _fileSaver.SaveFile(stream, name);
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            if (file.InputStream.Length == 0)
            {
                throw new NoDataException("There is no data in this stream!");
            }

            return SaveFile(file.InputStream, file.FileName);
        }
    }
}