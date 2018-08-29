using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Ec2;
using CloudStorageActivity.Azure;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;

namespace CloudComputingActivity.Virtual_Machines.Create_Virtual_Machine.Azure
{
    public class Ubuntu16 : CodeActivity
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
        /**/

        [Category("Input")]
        [DisplayName("Group Name")]
        [RequiredArgument]
        public InArgument<string> GroupName { get; set; }

        [Category("Input")]
        [DisplayName("Region")]
        [RequiredArgument]
        public InArgument<string> Region { get; set; }

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

                var azureVM = new AzureVM(SubscriptionId.Get(context), ClientSecret.Get(context)
                    , ClientId.Get(context), TenantId.Get(context));
                azureVM.CreateUbuntu16VM(GroupName.Get(context), VmName.Get(context),Region.Get(context),VmUserName.Get(context)
                    , VmUserPassword.Get(context));
                Response.Set(context, "Complete");
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
