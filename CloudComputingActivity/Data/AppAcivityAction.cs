using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorageActivity.Data
{
    public class AppAcivityAction
    {
        public String Service { set; get; }
        public String User { set; get; }
        public String Action { set; get; }
        public String SourcePath { set; get; }
        public String TargetPath { set; get; }
        public String Search { set; get; }
        public String Region { set; get; }
        public String AcessKey { set; get; }
        public String SecretKey { set; get; }
        public String BucketName { set; get; }

        public String AmiID { set; get; }
        public String KeyPairName { set; get; }
        public String InstanceType { set; get; }
        public String Host { set; get; }
        public String PrivateKeyPath { set; get; }
        public String[] Command { set; get; }
        public String ClientID { set; get; }
        public String TenantID { set; get; }
        public String Password { set; get; }
        public String GroupName { set; get; }
        public String VmName { set; get; }
        public String VmUserName { set; get; }
        
        public String VmUserPassword { set; get; }
        public String SubscriptionID { set; get; }
        public String Table { set; get; }
        public String[] Values { set; get; }
        public String[] Columns { set; get; }
        public String Db { set; get; }
    }
}
