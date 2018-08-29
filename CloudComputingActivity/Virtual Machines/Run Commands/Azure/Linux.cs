using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Ec2;
using CloudStorageActivity.Azure;
using CloudStorageActivity.Azure.Vm;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;
using Newtonsoft.Json;

namespace CloudComputingActivity.Virtual_Machines.Run_Commands.Azure
{
    public class Linux : CodeActivity
    {
        [Category("Credentials")]
        [DisplayName("Subscription Id")]
        [RequiredArgument]
        public InArgument<string> SubscriptionId { get; set; }

        [Category("Credentials")]
        [DisplayName("Client Secret")]
        [RequiredArgument]
        public InArgument<string> ClientSecret { get; set; }

        [Category("Credentials")]
        [DisplayName("Client Id")]
        [RequiredArgument]
        public InArgument<string> ClientId { get; set; }

        [Category("Credentials")]
        [DisplayName("Tenant Id")]
        [RequiredArgument]
        public InArgument<string> TenantId { get; set; }
        /**/
        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }

        [Category("Input")]
        [DisplayName("Group Name")]
        [RequiredArgument]
        public InArgument<string> GroupName { get; set; }

        [Category("Input")]
        [DisplayName("Commands (use ';' to separate multiple)")]
        [RequiredArgument]
        public InArgument<string> Commands { get; set; }

        [Category("Input")]
        [DisplayName("VM Name")]
        [RequiredArgument]
        public InArgument<string> VmName { get; set; }


        [Category("Input")]
        [DisplayName("VM User Name")]
        [RequiredArgument]
        public InArgument<string> VmUserName { get; set; }

        [Category("Input")]
        [DisplayName("VM User Password")]
        [RequiredArgument]
        public InArgument<string> VmUserPassword { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

                var azureVM = new AzureVMTerminalRunner(SubscriptionId.Get(context), ClientSecret.Get(context)
                    , ClientId.Get(context), TenantId.Get(context));
               var resonses= azureVM.RunUnixCommand(GroupName.Get(context), VmName.Get(context), Commands.Get(context).Split(';'), VmUserName.Get(context)
                    , VmUserPassword.Get(context));
                Response.Set(context, JsonConvert.SerializeObject(resonses));
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
