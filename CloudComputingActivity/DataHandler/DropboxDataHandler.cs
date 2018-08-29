using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CloudStorageActivity.Data;
using Dropbox.Api;
using Dropbox.Api.Files;
using DropboxRestAPI;

namespace CloudStorageActivity.DataHandler
{
    public class DropboxDataHandler : UniversalDataHandler
    {
        private  static String KEY = "qybsvssl4galwl5";
        private  static String SECRET = "corttgzrebsk6gw";
        private const string LoopbackHost = "http://localhost:8080/";
        private  readonly Uri RedirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private  readonly Uri JSRedirectUri = new Uri(LoopbackHost + "token");

        private DropboxClient DBClient;
        private String AppName = "FileNest";
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        public  DropboxDataHandler() {
            init();
        }
        private  void init()
        {
            var accessToken =  this.GetAccessToken();
            
            DropboxClientConfig CC = new DropboxClientConfig(AppName, 3);
            HttpClient HTC = new HttpClient();
            HTC.Timeout = TimeSpan.FromMinutes(60 * 4); 
            CC.HttpClient = HTC;
            DBClient = new DropboxClient(accessToken, CC);
        }
        public override  void Upload(FileItem from, FileItem to)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(from.Path)))
            {
                var response =  DBClient.Files.UploadAsync(to.Path, WriteMode.Overwrite.Instance, body: stream).Result;
            }

        }

        public override  void Download(FileItem from, FileItem to)
        {
            var response =  DBClient.Files.DownloadAsync(from.Path);
            var result =  response.Result.GetContentAsStreamAsync();
            Stream stream = new MemoryStream();
            result.Result.CopyTo(stream);
            DataUtils.Download(stream, to);
        }
        public override  void Delete(FileItem from)
        {
            var response = DBClient.Files.DeleteV2Async(from.Path).Result;
        }
        public override  void Copy(FileItem from, FileItem to)
        {

            var args = new RelocationArg(from.Path, to.Path);
           var response = DBClient.Files.CopyV2Async(args).Result;
        }
        public override  void Move(FileItem from, FileItem to)
        {
            var args = new RelocationArg(from.Path, to.Path);
            var response = DBClient.Files.MoveV2Async(args).Result;
        }
        public override FileItem[] ListFiles(FileItem from)
        {
            List<FileItem> mFileItems = new List<FileItem>();
            var args = new ListFolderArg(from.Path);
         var response=    DBClient.Files.ListFolderAsync(args);
            
            foreach (Metadata item in response.Result.Entries) {
                if (!item.IsDeleted)
                {

                    FileItem mFileItem = new FileItem();
                    mFileItem.Path = item.PathLower;
                    mFileItem.IsDirectory = item.IsFolder;
                    mFileItem.LastModified = item.AsFile.ClientModified.Ticks;
                    mFileItem.Size = (long)item.AsFile.Size;
                    mFileItems.Add(mFileItem);
                }
            }
            if (response.Result.HasMore)
            {
                response =  DBClient.Files.ListFolderContinueAsync(response.Result.Cursor);
                foreach (Metadata item in response.Result.Entries)
                {
                    if (!item.IsDeleted)
                    {

                        FileItem mFileItem = new FileItem();
                        mFileItem.Path = item.PathLower;
                        mFileItem.IsDirectory = item.IsFolder;
                        mFileItem.LastModified = item.AsFile.ClientModified.Ticks;
                        mFileItem.Size = (long)item.AsFile.Size;
                        mFileItems.Add(mFileItem);
                    }
                }
            }



           return mFileItems.ToArray();
        }
        public override  void CreateDir(FileItem from)
        {
            
            if (from.Path.EndsWith("/")) {
                from.Path = from.Path.Substring(0, from.Path.Length-1);
            }
            var folderArg = new CreateFolderArg(from.Path);
            var folder =  DBClient.Files.CreateFolderV2Async(folderArg).Result;

        }
        public override FileItem[] Search(FileItem from)
        {

            var args = new SearchArg(from.Path, from.SecondaryPath);
            var response =  DBClient.Files.SearchAsync(args);
            List<FileItem> mFileItems = new List<FileItem>();

            foreach (SearchMatch result in response.Result.Matches)
            {
                var item = result.Metadata;
                if (!item.IsDeleted)
                {

                    FileItem mFileItem = new FileItem();
                    mFileItem.Path = item.PathLower;
                    mFileItem.IsDirectory = item.IsFolder;
                    mFileItem.LastModified = item.AsFile.ClientModified.Ticks;
                    mFileItem.Size = (long)item.AsFile.Size;
                    mFileItems.Add(mFileItem);
                }
            }
            return mFileItems.ToArray();
        }
        private static class Keys { }
        
            private  void HandleOAuth2Redirect(HttpListener http)
            {
                var context =  http.GetContext();
            
            // We only care about request to RedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
            {
                context =  http.GetContext();
                }

            context.Response.ContentType = "text/html";

            // Respond with a page which runs JS and sends URL fragment as query string
            // to TokenRedirectUri.
            using (var file = File.OpenRead("CloudStorageActivity.index.html"))
            {
                file.CopyTo(context.Response.OutputStream);
            }


            context.Response.OutputStream.Close();
        }
            private OAuth2Response HandleJSRedirect(HttpListener http)
        {
            var context =  http.GetContext();
            
            // We only care about request to TokenRedirectUri endpoint.
            
            while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
            {
                context =  http.GetContext();
            }
            
            var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);
            
            var result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);

                return result;
            }
            private  string GetAccessToken()
            {
                var accessToken = ReadToken();

                if (string.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        Console.WriteLine("Waiting for credentials.");
                        var state = Guid.NewGuid().ToString("N");
                        var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, KEY, RedirectUri, state: state);
                        var http = new HttpListener();
                        http.Prefixes.Add(LoopbackHost);

                        http.Start();

                        System.Diagnostics.Process.Start(authorizeUri.ToString());
                    // Handle OAuth redirect and send URL fragment to local server using JS.
                    HandleOAuth2Redirect(http);
                        // Handle redirect from JS and process OAuth response.
                        var result =  HandleJSRedirect(http);
                    
                    if (result.State != state)
                        {
                        
                        // The state in the response doesn't match the state in the request.
                        return null;
                        }

                    SetForegroundWindow(GetConsoleWindow());

                    Console.WriteLine(accessToken);
                    accessToken = result.AccessToken;
                        var uid = result.Uid;
                        //Console.WriteLine("Uid: {0}", uid);
                    WriteToken(accessToken);
                    //Settings.Default.AccessToken = accessToken;
                    // Settings.Default.Uid = uid;

                    // Settings.Default.Save();
                }
                catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                        return null;
                    }
                }

                return accessToken;
            }
        
        private void WriteToken(String token)
        {
            System.IO.File.WriteAllText("rte", token);
        }
        private String ReadToken() {
            if (System.IO.File.Exists("rte"))
                return System.IO.File.ReadAllText(@"./rte");
            else
                return null;
        }
    }
}
