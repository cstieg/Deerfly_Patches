using System;
using System.IO;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Collections.Generic;
using Amazon.S3.Model;

namespace Deerfly_Patches.Modules.FileStorage.Amazon
{
    /// <summary>
    /// A file manager service to save files using Amazon S3 (Simple Storage Service)
    /// </summary>
    public class S3Service : IFileService
    {
        private string _connectionString;
        private string _domainName;
        private string _containerName;
        private string _accessKey = "";
        private string _secretKey = "";
        private AmazonS3Config config = new AmazonS3Config();
        private AmazonS3Client s3Client;

        /// <summary>
        /// Constructor for S3Service which configures the service
        /// </summary>
        /// <param name="containerName">The container where this service stores files.  Set by folder param in wrapper.</param>
        public S3Service(string domainName, string containerName = "")
        {
            _domainName = domainName;
            if (containerName != "")
            {
                SetFolder(containerName);
            }

            config.ServiceURL = "";
            s3Client = new AmazonS3Client(_accessKey, _secretKey, config);

            CreateBucketIfNotExists(_containerName);
        }

        public void SetFolder(string folder)
        {
            _containerName = _domainName + "/" + folder;
            _containerName = _containerName.Replace('/', '-').Replace('\\', '-').Replace('.', '-').Replace(' ', '-');
        }

        public S3Bucket GetBucket(string bucketName)
        {
            var bucketList = s3Client.ListBuckets().Buckets;
            return bucketList.Find(s => s.BucketName == bucketName);
        }

        public S3Bucket CreateBucketIfNotExists(string bucketName)
        {
            S3Bucket bucket = GetBucket(bucketName);
            if (bucket == null)
            {
                bucket = CreateBucket(bucketName);
            }
            return bucket;
        }

        public S3Bucket CreateBucket(string bucketName)
        {
            PutBucketRequest request = new PutBucketRequest()
            {
                BucketName = bucketName
            };
            s3Client.PutBucket(request);
            return GetBucket(bucketName);
        }

        /// <summary>
        /// An alias for UploadFile to fulfill the IFileManager interface
        /// </summary>
        /// <param name="stream">A Stream object containing the file data to be saved</param>
        /// <param name="name">The filename where the file is to be saved</param>
        /// <returns>The URL where the saved file can be accessed</returns>
        public string SaveFile(Stream stream, string name, bool timeStamped = false)
        {
            if (stream.Length != 0)
            {
                return UploadFile(stream, name);
            }
            throw new FileEmptyException();
        }

        /// <summary>
        /// Uploads a file to S3
        /// </summary>
        /// <param name="stream">A Stream object containing the file data to be uploaded</param>
        /// <param name="name">The filename where the file is to be uploaded</param>
        /// <returns>The URL where the uploaded file can be accessed</returns>
        public string UploadFile(Stream stream, string name)
        {
            try
            {
                string fileName = (DateTime.Now.Year.ToString() +
                                    DateTime.Now.Month.ToString() +
                                    DateTime.Now.Day.ToString() +
                                    DateTime.Now.Millisecond.ToString() +
                                    name);
                // Upload 
                using(TransferUtility transferUtility = new TransferUtility(s3Client))
                {
                    transferUtility.Upload(new TransferUtilityUploadRequest()
                    {
                        BucketName = _containerName,
                        Key = name,
                        InputStream = stream,
                        CannedACL = S3CannedACL.PublicRead
                    });
                }
                
                return _domainName + ".s3.amazonaws.com/" + fileName;

            }
            catch
            {
                throw new Exception("Failure to upload file");
            }

        }

        public List<S3Object> GetFiles()
        {
            ListObjectsRequest request = new ListObjectsRequest()
            {
                BucketName = _containerName
            };
            ListObjectsResponse response = s3Client.ListObjects(request);
            return response.S3Objects;
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="filePath">The URL of the file to delete</param>
        public void DeleteFile(string filePath)
        {
            int fileNameStart = filePath.LastIndexOf("/");
            string fileName = filePath.Substring(fileNameStart + 1);
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = _containerName,
                Key = fileName
            };
            s3Client.DeleteObject(request);
        }

        /// <summary>
        /// Deletes all files matching wildcard
        /// </summary>
        /// <param name="filePath">The URL of the file to delete containing wildcards</param>
        public void DeleteFilesWithWildcard(string filePath)
        {
            var fileList = GetFiles();
            fileList.ForEach(new Action<S3Object>(f =>
            {
                if (f.Key.Matches(filePath)) {
                    DeleteObjectRequest request = new DeleteObjectRequest()
                    {
                        BucketName = _containerName,
                        Key = f.Key
                    };
                    s3Client.DeleteObject(request);
                }

            }));
        }

    
    }
}
