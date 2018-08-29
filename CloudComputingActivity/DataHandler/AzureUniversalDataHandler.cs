using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CloudStorageActivity.DataHandler
{
    public class AzureUniversalDataHandler: UniversalDataHandler
    {
        private CloudStorageAccount storageAccount;
        private String storageconnectionstring;
        public AzureUniversalDataHandler(String user) {
            init(user);
        }
        private  void init(String storageconnectionstring)
        {
          //  string storageConnectionString = Environment.GetEnvironmentVariable(storageconnectionstring);
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageconnectionstring, out storageAccount))
            {
               }
        }
        private  Object BlobFromFileItem(FileItem to,bool isDir =false)
        {
            if (to.Path.StartsWith("/"))
                to.Path = to.Path.Substring(1, to.Path.Length - 1);
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(to.BucketName);
            var exists =  cloudBlobContainer.Exists();
            if (!exists)
                 cloudBlobContainer.Create();
            if(isDir)
               return cloudBlobContainer.GetDirectoryReference(to.Path);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(to.Path);
            return cloudBlockBlob;
        }
        public override void CreateDir(FileItem from)
        {
            CloudBlobDirectory directory = BlobFromFileItem(from,true) as CloudBlobDirectory;
            CloudBlockBlob blockBlob = directory.GetBlockBlobReference("temp");
            blockBlob.UploadFromByteArray(new byte[0], 0, 0);
         //   blockBlob.Delete();
        }
        public override  void Upload(FileItem from, FileItem to)
        {

            CloudBlockBlob cloudBlockBlob =  BlobFromFileItem( to) as CloudBlockBlob;
             cloudBlockBlob.UploadFromFile(from.Path);
        }
        public override  void Download(FileItem from, FileItem to)
        {
            
            CloudBlockBlob cloudBlockBlob =  BlobFromFileItem(from) as CloudBlockBlob; 

             cloudBlockBlob.DownloadToFile(to.Path, FileMode.Create);
        }
        public override  void Delete(FileItem from)
        {
            CloudBlockBlob cloudBlockBlob =  BlobFromFileItem(from) as CloudBlockBlob;

             cloudBlockBlob.Delete();
        }
        public override  void Copy(FileItem from, FileItem to)
        {
            CloudBlockBlob cloudBlockBlob =  BlobFromFileItem(from) as CloudBlockBlob;
            CloudBlockBlob targetBlob =  BlobFromFileItem(to) as CloudBlockBlob;
             targetBlob.StartCopy(cloudBlockBlob);
        }
        public override  void Move(FileItem from, FileItem to)
        {
            Copy(from, to);
            Delete(from);
        }
        public override FileItem[] ListFiles(FileItem from)
        {

            List<FileItem> mFileItems = new List<FileItem>();
            CloudBlockBlob cloudBlockBlob =  BlobFromFileItem(from) as CloudBlockBlob;
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results =  cloudBlockBlob.Container.ListBlobsSegmented(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    CloudBlockBlob file = item as CloudBlockBlob;
                    if (file.Name.StartsWith(from.Path)) {

                        FileItem mFileItem = new FileItem();
                        mFileItem.Path = file.Name;
                        mFileItem.IsDirectory = file.Name.EndsWith("/");
                        mFileItem.LastModified = file.Properties.LastModified.Value.Ticks;
                        mFileItem.Size = (long)file.Properties.Length;
                        mFileItems.Add(mFileItem);
                    }
                }
            } while (blobContinuationToken != null);
            return mFileItems.ToArray();
        }
      
        public override FileItem[] Search(FileItem from)
        {
            List<FileItem> mFileItems = new List<FileItem>();
            CloudBlockBlob cloudBlockBlob =  BlobFromFileItem(from) as CloudBlockBlob;
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results =  cloudBlockBlob.Container.ListBlobsSegmented(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    CloudBlockBlob file = item as CloudBlockBlob;
                    if (file.Name.StartsWith(from.Path) && file.Name.Contains(from.SecondaryPath))
                    {

                        FileItem mFileItem = new FileItem();
                        mFileItem.Path = file.Name;
                        mFileItem.IsDirectory = file.Name.EndsWith("/");
                        mFileItem.LastModified = file.Properties.LastModified.Value.Ticks;
                        mFileItem.Size = (long)file.Properties.Length;
                        mFileItems.Add(mFileItem);
                    }
                }
            } while (blobContinuationToken != null);
            return mFileItems.ToArray();
        }
    }
}
