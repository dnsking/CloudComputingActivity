using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;

namespace CloudComputingActivity.Cloud_Storage.Azure
{
    public class Delete : CodeActivity
    {
        [Category("Credentials")]
        [DisplayName("Connection String")]
        [RequiredArgument]
        public InArgument<string> AccessKey { get; set; }
        
        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }
        /**/

        [Category("Input")]
        [DisplayName("Source Path")]
        [RequiredArgument]
        public InArgument<string> SourcePath { get; set; }

        [Category("Input")]
        [DisplayName("Bucket Name")]
        [RequiredArgument]
        public InArgument<string> BucketName { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

              var  mUniversalDataHandler = new AzureUniversalDataHandler(AccessKey.Get(context));

                mUniversalDataHandler.Delete(Generate(SourcePath.Get(context), BucketName.Get(context)));
                Response.Set(context, "Complete");
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
        private FileItem Generate(String path, string bucketname = null, string searchkey = null)
        {
            FileItem mFileItem = new FileItem();
            mFileItem.Path = path;
            mFileItem.BucketName = bucketname;
            mFileItem.SecondaryPath = searchkey;
            return mFileItem;
        }
    }
}
