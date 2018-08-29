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
    public class Debian8 : Ubuntu16
    {
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

                var azureVM = new AzureVM(SubscriptionId.Get(context), ClientSecret.Get(context)
                    , ClientId.Get(context), TenantId.Get(context));
                azureVM.CreateDebian8VM(GroupName.Get(context), VmName.Get(context), Region.Get(context), VmUserName.Get(context)
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
