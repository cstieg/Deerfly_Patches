using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Web;


namespace Deerfly_Patches.Modules.FileStorage
{
    public class AzureBlobService : IFileSaver
    {
        private string _connectionString;
        private string _containerName;

        public AzureBlobService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
            ConfigureBlobContainer();
        }

        /// <summary>
        /// An alias for UploadFile fulfilling the IFileSaver interface
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string SaveFile(Stream stream, string name)
        {
            if (stream.Length != 0)
            {
                return UploadFile(stream, name);
            }
            return "";
        }

        public string UploadFile(Stream stream, string name)
        {
            try
            {
                string timeStampedFileName = (DateTime.Now.Year.ToString() +
                                   DateTime.Now.Month.ToString() +
                                   DateTime.Now.Day.ToString() +
                                   DateTime.Now.Millisecond.ToString() +
                                   name);


                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(_connectionString));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(_containerName);
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(timeStampedFileName);
                blob.UploadFromStream(stream);
                SetPublicContainerPermissions(blobContainer);
                return blob.Uri.AbsoluteUri;
            }
            catch
            {
                throw new AzureBlobException("Failure to upload file to blob");
            }
        }

        private void ConfigureBlobContainer()
        {
            try
            {
                // create blob container if does not exist and configure to be public
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(_connectionString));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(_containerName);
                if (blobContainer.CreateIfNotExists())
                {
                    SetPublicContainerPermissions(blobContainer);
                    //log.Information("Successfully created public blob storage container");
                }
            }
            catch
            {
                throw new AzureBlobException("Failure to create or configure blob storage service");
            }

        }

        public void SetPublicContainerPermissions(CloudBlobContainer blobContainer)
        {
            BlobContainerPermissions permissions = blobContainer.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            blobContainer.SetPermissions(permissions);
        }

    }
}