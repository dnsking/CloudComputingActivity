using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Ec2;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;
using CloudStorageActivity.GoogleCloud.CloudCompute;

namespace CloudComputingActivity.Virtual_Machines.Create_Virtual_Machine.Google_Cloud_Compute
{
    public class Linux : CodeActivity
    {

        [Category("Credentials")]
        [DisplayName("Email")]
        [RequiredArgument]
        public InArgument<string> Email { get; set; }
        [Category("Credentials")]
        [DisplayName("Client ID")]
        [RequiredArgument]
        public InArgument<string> AccessKey { get; set; }

        [Category("Credentials")]
        [DisplayName("Client Secret")]
        [RequiredArgument]
        public InArgument<string> SecretKey { get; set; }

        [Category("Credentials")]
        [DisplayName("Zone")]
        [RequiredArgument]
        public InArgument<string> Region { get; set; }

        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }
        /**/
        

        [Category("Input")]
        [DisplayName("Image Family")]
        [RequiredArgument]
        public InArgument<string> AmiID { get; set; }
        [Category("Input")]
        [DisplayName("VM Name")]
        [RequiredArgument]
        public InArgument<string> VmName { get; set; }


        [Category("Input")]
        [DisplayName("Image Project")]
        [RequiredArgument]
        public InArgument<string> ImgProject { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {

                var cmd = new CloudComputeConnector(Email.Get(context), AccessKey.Get(context), SecretKey.Get(context));

                cmd.InsertInstance(Region.Get(context), VmName.Get(context), AmiID.Get(context), ImgProject.Get(context));
                Response.Set(context, "Complete");
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
