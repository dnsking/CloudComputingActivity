using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudStorageActivity.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Storage.v1;

namespace CloudStorageActivity.DataHandler
{
    public class GCStorageDataHandler: UniversalDataHandler
    {
        private StorageService service;
        public GCStorageDataHandler(String email, String clientId, String clientSecret, String applicationName, String apiKey)
        {
            init(email,clientId, clientSecret, applicationName, apiKey);
        }
        private void init(String email, String clientId, String clientSecret, String applicationName, String apiKey)
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        },
         new[] { StorageService.Scope.DevstorageFullControl },
        email,
         CancellationToken.None, null).Result;
            service = new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
                ApiKey = apiKey
            });


        }
        
        public override void Upload(FileItem from, FileItem to)
        {
            var newObject = new Google.Apis.Storage.v1.Data. Object()
            {
                Bucket = to.BucketName,
                Name = to.Path
            };
         var request =   new Google.Apis.Storage.v1.ObjectsResource.InsertMediaUpload(service, newObject, to.BucketName, Data.DataUtils.GetStreamFromPath(from.Path),
                DataUtils.GetMimeType(System.IO.Path.GetFileName(from.Path)));
            request.Upload();
        }
        public override void Download(FileItem from, FileItem to)
        {
            Stream stream = new MemoryStream();

            new Google.Apis.Storage.v1.ObjectsResource.GetRequest(service, from.BucketName, from.Path).Download(stream);
            DataUtils.Download(stream, to);
        }
        public override void Delete(FileItem from)
        {
            new Google.Apis.Storage.v1.ObjectsResource.DeleteRequest(service, from.BucketName, from.Path).Execute();
        }
        public override void Copy(FileItem from, FileItem to)
        {
            new Google.Apis.Storage.v1.ObjectsResource.CopyRequest(service, new Google.Apis.Storage.v1.Data.Object()
            {
                Bucket = to.BucketName,
                Name = to.Path
            }, from.BucketName, from.Path, to.BucketName, to.Path).Execute();;
        }
        public override void Move(FileItem from, FileItem to)
        {
            Copy(from, to);
            Delete(from);
        }
        public override FileItem[] ListFiles(FileItem from)
        {
            Google.Apis.Storage.v1.Data.Objects objectListing= new Google.Apis.Storage.v1.ObjectsResource.ListRequest(service, from.BucketName).Execute();
            FileItem[] mFileItems = new FileItem[objectListing.Items.Count];
            int i = 0;
            foreach (Google.Apis.Storage.v1.Data.Object obj in objectListing.Items)
            {
                FileItem mFileItem = new FileItem();
                mFileItem.BucketName = obj.Bucket;
                mFileItem.Path = obj.Name;
                mFileItem.IsDirectory = obj.Name.EndsWith("/");
                mFileItem.LastModified = ((DateTime)obj.TimeCreated ).Ticks;
                mFileItem.Size = (long)obj.Size;
                mFileItems[i] = mFileItem;
                i++;
            }
            return mFileItems;
        }
        public override FileItem[] Search(FileItem from)
        {
            Google.Apis.Storage.v1.Data.Objects objectListing = new Google.Apis.Storage.v1.ObjectsResource.ListRequest(service, from.BucketName).Execute();
            FileItem[] mFileItems = new FileItem[objectListing.Items.Count];
            int i = 0;
            foreach (Google.Apis.Storage.v1.Data.Object obj in objectListing.Items)
            {

                if (obj.Name.Contains(from.SecondaryPath))
                {  FileItem mFileItem = new FileItem();
                    mFileItem.BucketName = obj.Bucket;
                    mFileItem.Path = obj.Name;
                    mFileItem.IsDirectory = obj.Name.EndsWith("/");
                    mFileItem.LastModified = ((DateTime)obj.TimeCreated).Ticks;
                    mFileItem.Size = (long)obj.Size;
                    mFileItems[i] = mFileItem;
                }
                i++;
            }
            return mFileItems;
        }
        public override void CreateDir(FileItem from)
        {
                var newObject = new Google.Apis.Storage.v1.Data.Object()
            {
                Bucket = from.BucketName,
                Name = from.Path
            };
            new Google.Apis.Storage.v1.ObjectsResource.InsertRequest(service, newObject, from.BucketName).Execute();
        }
    }
}
