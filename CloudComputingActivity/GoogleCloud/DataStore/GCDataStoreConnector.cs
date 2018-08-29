using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Aws.Rds;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Cloud.Datastore.V1;

namespace CloudStorageActivity.GoogleCloud.DataStore
{
    public class GCDataStoreConnector : RDSConnector
    {
        private DatastoreDb db;
        private DatastoreClient client;
        public GCDataStoreConnector(String dbname, string username, string password, string hostname, string port)
            :base( dbname,  username,  password,  hostname,  port)
        {
            
        }
    }
}
