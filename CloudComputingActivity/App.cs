using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorageActivity
{
    public class App
    {
        public static class Service
        {

            public static readonly String AwsS3 = "AwsS3";
            public static readonly String AzureBlob = "AzureBlob";
            public static readonly String GoogleDrive = "GDrive";
            public static readonly String DropBox = "DropBox";
            public static readonly String Box = "Box";
            public static readonly String AwsEC2 = "AwsEC2";
            public static readonly String AzureVm = "AzureVm";
            public static readonly String GoogleCloud = "GoogleCloud";
            public static readonly String AwsRDS = "AwsRDS";
            public static readonly String AzureDB = "AzureDB";
            public static readonly String GoogleCloudDb = "GoogleCloudDB";
        }
        public static class Commands
        {
            public static readonly String Service = "-S";
            public static readonly String User = "-U";
            public static readonly String Action = "-A";
            public static readonly String SourcePath = "-SP";
            public static readonly String TargetPath = "-TP";
            public static readonly String Search = "-S";
            public static readonly String Region = "-R";
            public static readonly String AcessKey = "-AK";
            public static readonly String BucketName = "-BK";
            public static readonly String SecretKey = "-SK";
            public static readonly String Help = "-H";

            public static readonly String AmiID = "-AMIID";
            public static readonly String KeyPairName = "-KPNAME";
            public static readonly String InstanceType = "-INSTANCETYPE";
            public static readonly String Host = "-HOST";
            public static readonly String PrivateKeyPath = "-PRIVATEKEYPATH";
            public static readonly String Command = "-COMMAND";

            public static readonly String ClientID = "-CLIENTID";
            public static readonly String TenantID = "-TENANTID";
            public static readonly String Password = "-PASSWORD";
            public static readonly String GroupName = "-GROUPNAME";
            public static readonly String VmName = "-VMNAME";
            public static readonly String VmUserName = "-VMUSERNAME";
            public static readonly String VmUserPassword = "-VMUSERPASSWORD";
            public static readonly String SubscriptionID = "-SUBSCRIPTIONID";


            public static readonly String Table = "-TABLE";
            public static readonly String Values = "-VALUES";
            public static readonly String Columns = "-COLUMNS";
            public static readonly String Db = "-DB";



        }
        public static class Actions
        {

            public static readonly String CreateDir = "CreateDir";
            public static readonly String Upload = "Upload";
            public static readonly String Download = "Download";
            public static readonly String Delete = "Delete";
            public static readonly String Move = "Move";
            public static readonly String Copy = "Copy";
            public static readonly String ListFiles = "ListFiles";
            public static readonly String Search = "Search";
            public static readonly String CreateInstance = "CreateInstance";
            public static readonly String VmCommand = "VmCommand";
            public static readonly String CreateLinuxVM = "CreateLinuxVM";
            public static readonly String CreateTable = "CreateTable";
        }
        public static class ConsoleOutput
        {

            public static readonly String UnknownCmd = "Unknown Command";
        }
    }
}
