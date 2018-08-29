using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Ec2;
using CloudStorageActivity.Azure;
using CloudStorageActivity.Azure.sql;
using CloudStorageActivity.Data;
using CloudStorageActivity.DataHandler;
using Newtonsoft.Json;

namespace CloudComputingActivity.Databases.Azure
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

                var azureSqlConnector = new AzureSqlConnector(Db.Get(context), User.Get(context), Password.Get(context), Host.Get(context));
                var value = azureSqlConnector.UpdateInto(Table.Get(context), Columns.Get(context).Split(';')
                    , Values.Get(context).Split(';'), SelectKey.Get(context), SelectValue.Get(context)); ;
                Response.Set(context, JsonConvert.SerializeObject(new { item = value }));
            }
            catch (Exception e)
            {
                Error.Set(context, e.Message);
            }
        }
    }
}
