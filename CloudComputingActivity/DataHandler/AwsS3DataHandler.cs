using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using CloudStorageActivity.Data;

namespace CloudStorageActivity.DataHandler
{
    public class AwsS3DataHandler : UniversalDataHandler
    {
        private IAmazonS3 s3;
        private String region;
        public AwsS3DataHandler(String myaccesskey,String mysecretkey, String region) {
            var awsCredentials = new BasicAWSCredentials(myaccesskey, mysecretkey);
            s3 = new AmazonS3Client(awsCredentials, Regions.RegionEndpointFromName(region));
        }
        private  void  CheckBucket(String bucketName) {
             s3.EnsureBucketExists(bucketName);
        }
        public override  void Upload(FileItem from, FileItem to)
        {
            CheckBucket(to.BucketName);
            to.Path = CheckPath(to.Path);
            var mPutObjectRequest = new PutObjectRequest();
            mPutObjectRequest.BucketName = to.BucketName;
            mPutObjectRequest.Key = to.Path;
            
            mPutObjectRequest.InputStream = Data.DataUtils.GetStreamFromPath(from.Path);
            s3.PutObject(mPutObjectRequest);
        }
        public override  void Download(FileItem from, FileItem to)
        {
            from.Path = CheckPath(from.Path);

            var mGetObjectRequest = new GetObjectRequest();
            mGetObjectRequest.BucketName = from.BucketName;
            mGetObjectRequest.Key = from.Path;

            GetObjectResponse response = s3.GetObject(mGetObjectRequest);
            /*FileItem mFileItem = new FileItem();
            mFileItem.BucketName = response.BucketName;
            mFileItem.Path = response.Key;
            mFileItem.LastModified = response.LastModified.Ticks;
            mFileItem.Size = response.ContentLength;*/
            Stream stream = new MemoryStream();
            response.ResponseStream.CopyTo(stream);
            DataUtils.Download(stream, to);
           // mFileItem.Content = DataUtils.ReadFully(response.ResponseStream);
        }
        
        public override  void Delete(FileItem from)
        {
            from.Path = CheckPath(from.Path);
            s3.DeleteObject(from.BucketName, from.Path);
        }
        public override  void Copy(FileItem from, FileItem to)
        {

            CheckBucket(to.BucketName);
              s3.CopyObject(from.BucketName, from.Path, to.BucketName, to.Path);
        }
        public override void Move(FileItem from, FileItem to)
        {
            from.Path = CheckPath(from.Path);
            to.Path = CheckPath(to.Path);
            Copy(from, to);
            Delete(from);
        }
        public override FileItem[] ListFiles(FileItem from)
        {

            ListObjectsResponse objectListing =  s3.ListObjects(from.BucketName, from.Path);
            FileItem[] mFileItems = new FileItem[objectListing.S3Objects.Count];
            int i = 0;
            foreach (S3Object mS3Object in objectListing.S3Objects)
            {
                FileItem mFileItem = new FileItem();
                mFileItem.BucketName = mS3Object.BucketName;
                mFileItem.Path = mS3Object.Key;
                mFileItem.IsDirectory = mS3Object.Key.EndsWith("/");
                mFileItem.LastModified = mS3Object.LastModified.Ticks;
                mFileItem.Size = mS3Object.Size;
                mFileItems[i] = mFileItem;
                i++;
            }
          return mFileItems;
        }
        private String CheckPath(String path)
        {
            if (path.StartsWith("/"))
                path = path.Substring(1, path.Length - 1);

            return path;
        }
        public override  void CreateDir(FileItem from)
        {
            CheckBucket(from.BucketName);
            var mPutObjectRequest = new PutObjectRequest();
            mPutObjectRequest.BucketName = from.BucketName;
            from.Path = CheckPath(from.Path);
            if (!from.Path.EndsWith("/"))
                from.Path = from.Path+"/";

            Console.WriteLine(from.Path);

            mPutObjectRequest.Key = from.Path;
            //mPutObjectRequest.Key = "mii/asa/";
            mPutObjectRequest.InputStream = new MemoryStream(new byte[0]);
            s3.PutObject(mPutObjectRequest);
        }
        public void CheckFolders(String Path) {
            
        }
        public override FileItem[] Search(FileItem from)
        {
            ListObjectsResponse objectListing = s3.ListObjects(from.BucketName, from.Path);
            var mFileItems = new List<FileItem>();
            foreach (S3Object mS3Object in objectListing.S3Objects)
            {
                if (mS3Object.Key.Contains(from.SecondaryPath))
                {

                    FileItem mFileItem = new FileItem();
                    mFileItem.BucketName = mS3Object.BucketName;
                    mFileItem.Path = mS3Object.Key;
                    mFileItem.LastModified = mS3Object.LastModified.Ticks;
                    mFileItem.Size = mS3Object.Size;
                    mFileItems.Add(mFileItem);
                }
            }

           return  mFileItems.ToArray();
        }
        public static class Regions
        {
            public static string USEast1 = "USEast1";
            public static string USEast2 = "USEast2";
            public static string USWest1 = "USWest1";
            public static string USWest2 = "USWest2";
            public static string EUWest1 = "EUWest1";
            public static string EUWest2 = "EUWest2";
            public static string EUWest3 = "EUWest3";
            public static string EUCentral1 = "EUCentral1";
            public static string APNortheast1 = "APNortheast1";
            public static string APNortheast2 = "APNortheast2";
            public static string APSouth1 = "APSouth1";
            public static string APSoutheast1 = "APSoutheast1";
            public static string APSoutheast2 = "APSoutheast2";
            public static string SAEast1 = "SAEast1";
            public static string USGovCloudWest1 = "USGovCloudWest1";
            public static string CNNorth1 = "CNNorth1";
            public static string CNNorthWest1 = "CNNorthWest1";
            public static string CACentral1 = "CACentral1";
            public static String[] AllRegions = new String[] {
             USEast1         ,
             USEast2         ,
             USWest1         ,
             USWest2         ,
             EUWest1         ,
             EUWest2         ,
             EUWest3         ,
             EUCentral1      ,
             APNortheast1    ,
             APNortheast2    ,
             APSouth1        ,
             APSoutheast1    ,
             APSoutheast2    ,
             SAEast1         ,
             USGovCloudWest1 ,
             CNNorth1        ,
             CNNorthWest1    ,
             CACentral1      ,
        };
            public static RegionEndpoint[] AllActualRegions = new RegionEndpoint[] {
             RegionEndpoint.USEast1         ,
             RegionEndpoint.USEast2         ,
             RegionEndpoint.USWest1         ,
             RegionEndpoint.USWest2         ,
             RegionEndpoint.EUWest1         ,
             RegionEndpoint.EUWest2         ,
             RegionEndpoint.EUWest3         ,
             RegionEndpoint.EUCentral1      ,
             RegionEndpoint.APNortheast1    ,
             RegionEndpoint.APNortheast2    ,
             RegionEndpoint.APSouth1        ,
             RegionEndpoint.APSoutheast1    ,
             RegionEndpoint.APSoutheast2    ,
             RegionEndpoint.SAEast1         ,
             RegionEndpoint.USGovCloudWest1 ,
             RegionEndpoint.CNNorth1        ,
             RegionEndpoint.CNNorthWest1    ,
             RegionEndpoint.CACentral1      ,
        };
            public static RegionEndpoint RegionEndpointFromName(String name)
            {
                return AllActualRegions[AllRegions.ToList().IndexOf(name)];
            }
        }

    }
}
