using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorageActivity.Data
{
    public class FileItem
    {
        public String  Service { set; get; }
        public String SecondaryPath { set; get; }
        public String Path { set; get; }
        public String BucketName { set; get; }
        public long Size { set; get; }
        public long LastModified { set; get; }
        public Boolean IsDirectory { set; get; }
        public byte[] Content { set; get; }
    }
}
