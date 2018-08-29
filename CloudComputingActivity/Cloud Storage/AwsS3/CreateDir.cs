using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;

namespace CloudComputingActivity.Cloud_Storage.AwsS3
{
    public class CreateDir : CodeActivity
    {
        [Category("Credentials")]
        [DisplayName("Access Key")]
        [RequiredArgument]
        public InArgument<string> AccessKey { get; set; }

        [Category("Credentials")]
        [DisplayName("Secret Key")]
        [RequiredArgument]
        public InArgument<string> SecretKey { get; set; }

        [Category("Credentials")]
        [DisplayName("Region")]
        [RequiredArgument]
        public InArgument<string> Region { get; set; }

        [Category("Input")]
        [DisplayName("Source Path")]
        [RequiredArgument]
        public InArgument<string> SourcePath { get; set; }

        [Category("Input")]
        [DisplayName("Bucket Name")]
        [RequiredArgument]
        public InArgument<string> BucketName { get; set; }


        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try {

                var mUniversalDataHandler = new AwsS3DataHandler(AccessKey.Get(context), SecretKey.Get(context), Region.Get(context));

                mUniversalDataHandler.CreateDir(Generate(SourcePath.Get(context), BucketName.Get(context)));
                Response.Set(context, "Complete");
            }
            catch(Exception e) {
                Error.Set(context, e.Message);
            }
        }
        private  FileItem Generate(String path, string bucketname = null, string searchkey = null)
        {
            FileItem mFileItem = new FileItem();
            mFileItem.Path = path;
            mFileItem.BucketName = bucketname;
            mFileItem.SecondaryPath = searchkey;
            return mFileItem;
        }
    }
}
