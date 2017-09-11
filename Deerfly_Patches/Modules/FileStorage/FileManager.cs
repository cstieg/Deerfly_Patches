using System;
using System.IO;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    /// <summary>
    /// A wrapper for the default file storage service
    /// </summary>
    public class FileManager : IFileManager
    {
        protected IFileManager _fileManager;
        protected string _storageService;
        protected string _folder;
        
        /// <summary>
        /// Constructor for FileManager which selects the file storage service to be used
        /// </summary>
        /// <param name="folder">The folder in which the files are to be saved</param>
        /// <param name="storageService">A string containing the name of the file storage service: "fileSystem", "AzureBlob"</param>
        public FileManager(string folder, string storageService = "")
        {
            _folder = folder;

            // Get default storage service from RouteConfig if not specifically provided
            if (storageService == "")
            {
                _storageService = RouteConfig.storageService;
            }
            else
            {
                _storageService = storageService;
            }

            // Set storage service to be used
            switch (_storageService)
            { 
                /* uncomment to add Azure Blob service
                case "AzureBlob":
                    if (folder == "")
                    {
                        throw new Exception("Container is required for Azure Blob Service");
                    }
                    _fileManager = new AzureBlobService("AzureStorageConnectionString", folder);
                    break;
                */

                case "fileSystem":
                    if (folder != "")
                    {
                        folder = "/" + folder;
                    }
                    _fileManager = new FileSystemService(RouteConfig.contentFolder + folder);
                    break;
            }
        }

        /// <summary>
        /// Saves a posted file to the selected storage service
        /// </summary>
        /// <param name="file">The file to be saved, derived from a POST request</param>
        /// <returns>The URL by which the saved file is accessible</returns>
        public string SaveFile(HttpPostedFileBase file, bool timeStamped = true)
        {
            if (file.InputStream.Length == 0)
            {
                throw new NoDataException("There is no data in this stream!");
            }

            return SaveFile(file.InputStream, file.FileName, timeStamped);
        }

        /// <summary>
        /// Saves a file stream to the selected storage service
        /// </summary>
        /// <param name="stream">The stream containing the file data to be saved</param>
        /// <param name="name">The filename by which to save the file</param>
        /// <returns></returns>
        public string SaveFile(Stream stream, string name, bool timeStamped = true)
        {
            if (timeStamped)
            {
                // Timestamp the filename to prevent collisions
                name = GetTimeStampedFileName(name);
            }

            if (stream.Length == 0)
            {
                throw new NoDataException("There is no data in this stream!");
            }

            // Replace spaces with underscores for HTML access
            name = name.Replace(' ', '_');

            return _fileManager.SaveFile(stream, name);
        }

        /// <summary>
        /// Deletes a file from the file storage service
        /// </summary>
        /// <param name="filePath">The name of the file to be deleted</param>
        public void DeleteFile(string filePath)
        {
            _fileManager.DeleteFile(filePath);
        }

        /// <summary>
        /// Deletes all files which match the wildcard pattern from the file storage service
        /// </summary>
        /// <param name="filePath">The name of the files to be deleted including wildcards</param>
        public void DeleteFilesWithWildcard(string filePath)
        {
            _fileManager.DeleteFilesWithWildcard(filePath);
        }

        public string GetTimeStampedFileName(string name)
        {
            return (DateTime.Now.Year.ToString("D4") +
                   DateTime.Now.Month.ToString("D2") +
                   DateTime.Now.Day.ToString("D2") +
                   DateTime.Now.Hour.ToString("D2") +
                   DateTime.Now.Minute.ToString("D2") +
                   DateTime.Now.Second.ToString("D2") +
                   "-" + name);
        }
    }
}