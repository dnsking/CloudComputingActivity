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
using Newtonsoft.Json;

namespace CloudComputingActivity.Databases.Aws_RDS
{
    public class Update : CodeActivity
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
        [Category("Input")]
        [DisplayName("Port")]
        [RequiredArgument]
        public InArgument<string> Port { get; set; }
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
        [DisplayName("Values (use ';' to separate multiple)")]
        [RequiredArgument]
        public InArgument<string> Values { get; set; }
        [Category("Input")]
        [DisplayName("Select Key")]
        [RequiredArgument]
        public InArgument<string> SelectKey { get; set; }
        [Category("Input")]
        [DisplayName("Select Value")]
        [RequiredArgument]
        public InArgument<string> SelectValue { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {

                var cmd = new RDSConnector(Db.Get(context), User.Get(context), Password.Get(context), Host.Get(context), Port.Get(context));
                var value = cmd.UpdateInto(Table.Get(context), Columns.Get(context).Split(';'), Values.Get(context).Split(';'), SelectKey.Get(context), SelectValue.Get(context)); ;
                Response.Set(context, JsonConvert.SerializeObject(new { item = value }));
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
