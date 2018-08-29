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
using Newtonsoft.Json;

namespace CloudComputingActivity.Virtual_Machines.Run_Commands.AWS_EC2
{
    public class Linux : CodeActivity
    {

        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }
        /**/

        [Category("Input")]
        [DisplayName("Commands (use ';' to separate multiple)")]
        [RequiredArgument]
        public InArgument<string> Commands { get; set; }

        [Category("Input")]
        [DisplayName("Private Key Path")]
        [RequiredArgument]
        public InArgument<string> PrivateKeyPath { get; set; }

        [Category("Input")]
        [DisplayName("Host")]
        [RequiredArgument]
        public InArgument<string> Host { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

             var resonses=   EC2TerminalRunner.RunUnixCommand(Host.Get(context), PrivateKeyPath.Get(context), Commands.Get(context).Split(';'));
                Response.Set(context, JsonConvert.SerializeObject( resonses));
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
