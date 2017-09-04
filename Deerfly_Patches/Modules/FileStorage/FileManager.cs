using System;
using System.IO;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileManager : IFileManager
    {
        protected IFileManager _fileManager;
        protected string _storageService;
        protected string _folder;
        
        public FileManager(string folder, string storageService = "")
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
                    _fileManager = new AzureBlobService("AzureStorageConnectionString", folder);
                    break;

                case "fileSystem":
                    if (folder != "")
                    {
                        folder = "/" + folder;
                    }
                    _fileManager = new FileSystemService(RouteConfig.contentFolder + folder);
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
            return _fileManager.SaveFile(stream, name);
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            if (file.InputStream.Length == 0)
            {
                throw new NoDataException("There is no data in this stream!");
            }

            return SaveFile(file.InputStream, file.FileName);
        }

        public void DeleteFile(string filePath)
        {
            _fileManager.DeleteFile(filePath);
        }

        public void DeleteFilesWithWildcard(string filePath)
        {
            _fileManager.DeleteFilesWithWildcard(filePath);
        }
    }
}