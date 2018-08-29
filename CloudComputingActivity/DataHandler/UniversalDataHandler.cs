using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Data;

namespace CloudStorageActivity.DataHandler
{
    public class UniversalDataHandler
    {
        
        public class FilesHolder 
        {

            public FileItem[] Files { set; get; }
        }
        public virtual void Upload(FileItem from, FileItem to)
        {
        }
        public virtual void Download(FileItem from, FileItem to)
        {
        }
        public virtual void Delete(FileItem from)
        {
        }

        public virtual void Copy(FileItem from, FileItem to)
        {
        }

        public virtual void Move(FileItem from, FileItem to)
        {
        }

        public virtual FileItem[] ListFiles(FileItem from)
        {
            return new FileItem[] { };
        }
        public virtual void CreateDir(FileItem from)
        {
        }
        public virtual FileItem[] Search(FileItem from)
        {
            return new FileItem[] { };
        }
    }

}
