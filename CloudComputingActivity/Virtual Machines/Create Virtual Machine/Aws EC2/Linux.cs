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

namespace CloudComputingActivity.Virtual_Machines.Create_Virtual_Machine.Aws_EC2
{
    public class Linux : CodeActivity
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

        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }
        /**/

        [Category("Input")]
        [DisplayName("Instance Type")]
        [RequiredArgument]
        public InArgument<string> InstanceType { get; set; }

        [Category("Input")]
        [DisplayName("Key Pair Name")]
        [RequiredArgument]
        public InArgument<string> KeyPairName { get; set; }

        [Category("Input")]
        [DisplayName("Ami ID")]
        [RequiredArgument]
        public InArgument<string> AmiID { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

                var cmd = new EC2Connector(AccessKey.Get(context), SecretKey.Get(context), Region.Get(context));

                cmd.CreateInstance(AmiID.Get(context), KeyPairName.Get(context), InstanceType.Get(context));
                Response.Set(context, "Complete");
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
