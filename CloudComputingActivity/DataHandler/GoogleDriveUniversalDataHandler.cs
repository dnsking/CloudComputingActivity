using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudStorageActivity.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace CloudStorageActivity.DataHandler
{
    public class GoogleDriveUniversalDataHandler : UniversalDataHandler
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "CustomActivity";
        private DriveService service;

        public GoogleDriveUniversalDataHandler() {
            init();
        }
        private  void init()
        {
            UserCredential credential;
            using (var stream =
                new FileStream("CloudStorageActivity.credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                var result =  GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
                    credential = result.Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

             service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

        }
        public override  void Upload(FileItem from, FileItem to)
        {

            var parent = "root";
            var ids = CheckFolder(to.Path, false,true);
            if(ids.Count>0)
               parent = ids.Last();

            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
            body.Parents = new List<String>() { parent };
            body.Name = FileNameFromPath(to.Path) ;
            //body.MimeType = DataUtils.GetMimeType(Path.GetFileName(to.Path));
            var request = service.Files.Create(body, DataUtils.GetStreamFromPath(from.Path), DataUtils.GetMimeType(Path.GetFileName(to.Path)));
             request.Upload();
        }
        public override  void Download(FileItem from, FileItem to)
        {
            var ids = CheckFolder(from.Path, true);
            var request = service.Files.Get(ids.Last());
            var stream = new System.IO.MemoryStream();
             request.Download(stream);
            DataUtils.Download(stream, to);
        }
        public override  void Delete(FileItem from)
        {

            List<String> ids = CheckFolder(from.Path, false);
            service.Files.Delete(ids.Last()).ExecuteAsync();
        }
        public override  void Copy(FileItem from, FileItem to)
        {
           var ids = CheckFolder(to.Path, true);
            service.Files.Copy(GFileFromPath(from.Path), ids.Last()).Execute();

        }
        private String FileNameFromPath(string Path)
        {
            String[] dirs = Path.Split('/');
            if (dirs.Length > 0)
            {
                return dirs.Last();
            }
            else
            {
                return Path;
            }
        }
        private Google.Apis.Drive.v3.Data.File GFileFromPath(string Path) {

            String[] dirs = Path.Split('/');
            if (dirs.Length > 0)
            {  List<String> ids = CheckFolder(Path, false);
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                if (ids.Count > 1)
                {

                    body.Parents = new List<String>() { ids[dirs.Length - 2] };
                }
                else
                {
                    body.Parents = new List<String>() { "root" };
                }
                body.Name = dirs.Last();
                return body;
            }
            else {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();

                body.Parents = new List<String>() { "root" };
                body.Name = Path;
                return body;
            }
        }
        public override void Move(FileItem from, FileItem to)
        {
            Copy(from, to);
            Delete(from);
        }
        public override FileItem[] ListFiles(FileItem from)
        {

            List<FileItem> mFileItems = new List<FileItem>();
            var request = service.Files.List();
            request.Q = "parents in '" + from.Path + "'";
             var files =  request.Execute();

            foreach(Google.Apis.Drive.v3.Data.File file in files.Files)
            {
                FileItem mFileItem = new FileItem();
                mFileItem.Path = file.Parents[0]+"/"+ file.OriginalFilename;
                mFileItem.IsDirectory = file.OriginalFilename.EndsWith("/");
                mFileItem.LastModified = file.ModifiedTime.Value.Ticks;
                mFileItem.Size = (long)file.Size;
                mFileItems.Add(mFileItem);
            }
          return mFileItems.ToArray();
        }
        private List<String> CheckFolder(string path,Boolean isFile,bool skip =false)
        {
            List<String> Parents = new List<string>();
           String[] dirs = path.Split('/');
            if ( skip) {
                dirs = path.Split('/').ToList().GetRange(0, dirs.Length-1).ToArray();
            }
            String id = "root";
            foreach(String dir in dirs)
            {
                if (dir.Length > 0)
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = dir
                    };
                    if(!isFile)
                    fileMetadata.MimeType = "application/vnd.google-apps.folder";


                    if (id != null)
                        fileMetadata.Parents = new List<String>() { id };

                    var prevId = Exists(id, dir);
                    if (prevId == null)
                    {
                        Console.WriteLine("Creating...");
                        var request = service.Files.Create(fileMetadata);
                        request.Fields = "id";
                        var file = request.Execute();
                        id = file.Id;
                    }
                    else { id = prevId; }

                    Console.WriteLine("id");
                    Console.WriteLine(id);
                    Parents.Add(id);
                }
            }
            return Parents;
        }
        public override  void CreateDir(FileItem from)
        {
            Console.WriteLine("Path");
            Console.WriteLine(from.Path);
            Console.WriteLine("Parents");
            Console.WriteLine(Path.GetPathRoot(from.Path));
            Console.WriteLine("Children");
            Console.WriteLine(Path.GetFileName(from.Path));
            CheckFolder(from.Path,false);
            /*
            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
            body.Parents = new List<String>() { Path.GetPathRoot(from.Path) };
            body.Name = Path.GetFileName(from.Path);
            body.MimeType = "application/vnd.google-apps.folder";
            var request = service.Files.Create(body);
             request.Execute();*/
        }
        public  String Exists(String parent , String  name)
        {
            Console.WriteLine("Searching for...");
            Console.WriteLine(parent+":"+name);
            List<FileItem> mFileItems = new List<FileItem>();
            var request = service.Files.List();
            if(parent!=null)
            request.Q = "parents in '" + parent + "' and name = '" + name + "'";
            else
                request.Q = "name = '" + name + "'";
            var files = request.Execute();
            if (files.Files.Count > 0)
            {
                return files.Files[0].Id;
            }
            return null;
        }
        public override FileItem[] Search(FileItem from)
        {

            List<FileItem> mFileItems = new List<FileItem>();
            var request = service.Files.List();
            request.Q = "parents in '" + from.Path + "' and title contains '"+from.SecondaryPath+"'";
            var files =  request.Execute();

            foreach (Google.Apis.Drive.v3.Data.File file in files.Files)
            {
                FileItem mFileItem = new FileItem();
                mFileItem.Path = file.Parents[0] + "/" + file.OriginalFilename;
                mFileItem.IsDirectory = file.OriginalFilename.EndsWith("/");
                mFileItem.LastModified = file.ModifiedTime.Value.Ticks;
                mFileItem.Size = (long)file.Size;
                mFileItems.Add(mFileItem);
            }
            return mFileItems.ToArray();
        }
    }
}
