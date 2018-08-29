using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box.V2.Models;

namespace CloudStorageActivity.Data
{
    public class DataUtils
    {
        public static bool IsDir(String path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        public static Stream GetStreamFromPath(String path)
        {
           return new System.IO.FileStream(path,
                        System.IO.FileMode.Open);
        }
        public static void Download(Stream input,FileItem to)
        {
            var fileStream = File.Create(to.Path);
            input.Seek(0, SeekOrigin.Begin);
            input.CopyTo(fileStream);
            fileStream.Close();
        }
        public static BoxFileRequest FileItemToBoxFileRequest(FileItem to)
        {

            BoxFileRequest req = new BoxFileRequest();
            req.Name = Path.GetFileName(to.Path);
            req.Parent = new BoxRequestEntity() { Id = Path.GetPathRoot(to.Path) };
            return req;
        }
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
