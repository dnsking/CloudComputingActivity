using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Ec2;
using CloudStorageActivity.Aws.Rds;
using CloudStorageActivity.Azure;
using CloudStorageActivity.Azure.sql;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;
using CloudStorageActivity.GoogleCloud.DataStore;
using Newtonsoft.Json;

namespace CloudComputingActivity.Databases.Google_Cloud_Compute
{
    public class Delete : CodeActivity
    {
        [Category("Input")]
        [DisplayName("Database")]
        [RequiredArgument]
        public InArgument<string> Db { get; set; }

        [Category("Input")]
        [DisplayName("User")]
        [RequiredArgument]
        public InArgument<string> User { get; set; }

        [Category("Input")]
        [DisplayName("Password")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        [Category("Input")]
        [DisplayName("Host")]
        [RequiredArgument]
        public InArgument<string> Host { get; set; }
        /**/
        [Category("Output")]
        [DisplayName("Response")]
        public InArgument<string> Response { get; set; }

        [Category("Output")]
        [DisplayName("Error")]
        public InArgument<string> Error { get; set; }

        [Category("Input")]
        [DisplayName("Table")]
        [RequiredArgument]
        public InArgument<string> Table { get; set; }
        [Category("Input")]
        [DisplayName("Columns (use ';' to separate multiple)")]
        [RequiredArgument]
        public InArgument<string> Columns { get; set; }
        [Category("Input")]
        [DisplayName("Columns Values (use ';' to separate multiple)")]
        [RequiredArgument]
        public InArgument<string> ColumnsValues { get; set; }
        [Category("Input")]
        [DisplayName("Port")]
        [RequiredArgument]
        public InArgument<string> Port { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

                var cmd = new GCDataStoreConnector(Db.Get(context), User.Get(context), Password.Get(context), Host.Get(context), Port.Get(context));
                var value = cmd.Delete(Table.Get(context), Columns.Get(context).Split(';'), ColumnsValues.Get(context).Split(';')); ;
                Response.Set(context, JsonConvert.SerializeObject(new { item = value }));
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
