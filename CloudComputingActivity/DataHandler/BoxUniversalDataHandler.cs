using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box.V2;
using Box.V2.Config;
using Box.V2.Models;
using CloudStorageActivity.Data;
using Box.V2.Models.Request;

namespace CloudStorageActivity.DataHandler
{
    public class BoxUniversalDataHandler: UniversalDataHandler
    {

        private static String KEY = "qybsvssl4galwl5";
        private static String SECRET = "corttgzrebsk6gw";
        private const string LoopbackHost = "http://localhost:8080/";
        private readonly Uri RedirectUri = new Uri(LoopbackHost + "auth");
        public BoxUniversalDataHandler(){
            init();}
        private BoxClient client;
        private async void init()
        {
            var config = new BoxConfig(KEY, SECRET, RedirectUri);
            client = new BoxClient(config);
           // string authCode = await OAuth2Sample.GetAuthCode(config.AuthCodeUri, config.RedirectUri);
          //  await client.Auth.AuthenticateAsync(authCode);
        }

        public override async void Upload(FileItem from, FileItem to)
        {
            BoxFileRequest req = new BoxFileRequest();
            req.Name = Path.GetFileName(to.Path);
            req.Parent = new BoxRequestEntity() { Id = Path.GetPathRoot(to.Path) };
            var newFile = await client.FilesManager.UploadAsync(req, DataUtils.GetStreamFromPath(from.Path));
        }
        public override async void Download(FileItem from, FileItem to)
        {
            Stream stream = await client.FilesManager.DownloadStreamAsync(from.Path);
            DataUtils.Download(stream, to);
        }
        public override async void Delete(FileItem from)
        {
            await client.FilesManager.DeleteAsync(from.Path);
        }
        public override async void Copy(FileItem from, FileItem to)
        {
            BoxFileRequest req = new BoxFileRequest();
            req.Name = Path.GetFileName(to.Path);
            req.Parent = new BoxRequestEntity() { Id = Path.GetPathRoot(to.Path) };
            BoxFile request = await client.FilesManager.CopyAsync(req);
        }
        public override void Move(FileItem from, FileItem to)
        {
            Copy(from, to);
            Delete( from);
        }
        public override  FileItem[] ListFiles(FileItem from)
        {
            List<FileItem> mFileItems = new List<FileItem>();
            var items =  client.FoldersManager.GetFolderItemsAsync(from.Path, 500);
            foreach (BoxItem mBoxItem in items.Result.Entries) {

                FileItem mFileItem = new FileItem();
                mFileItem.Path = mBoxItem.Parent + "/" + mBoxItem.Name;
                //Todo Fix
                mFileItem.IsDirectory = mBoxItem.Name.EndsWith("/");
                mFileItem.LastModified = mBoxItem.ModifiedAt.Value.Ticks;
                mFileItem.Size = (long)mBoxItem.Size;
                mFileItems.Add(mFileItem);
            }
         return mFileItems.ToArray();
        }
        private  async void FetchFiles(int offset, List<FileItem> mFileItems, FileItem from,int count)
        {

            var items = await client.FoldersManager.GetFolderItemsAsync(from.Path, 500, offset);
            foreach (BoxItem mBoxItem in items.Entries)
            {

                FileItem mFileItem = new FileItem();
                mFileItem.Path = mBoxItem.Parent + "/" + mBoxItem.Name;
                //Todo Fix
                mFileItem.IsDirectory = mBoxItem.Name.EndsWith("/");
                mFileItem.LastModified = mBoxItem.ModifiedAt.Value.Ticks;
                mFileItem.Size = (long)mBoxItem.Size;
                mFileItems.Add(mFileItem);
            }
            count += items.Entries.Count();
            if (items.TotalCount> count) {
                FetchFiles(count, mFileItems, from, count);
            }

        }

        public override void CreateDir(FileItem from)
        {
        }
        public override FileItem[] Search(FileItem from)
        {


            List<FileItem> mFileItems = new List<FileItem>();
            var items =  client.SearchManager.SearchAsync(from.SecondaryPath, 50);
            foreach (BoxItem mBoxItem in items.Result.Entries)
            {

                FileItem mFileItem = new FileItem();
                mFileItem.Path = mBoxItem.Parent + "/" + mBoxItem.Name;
                //Todo Fix
                mFileItem.IsDirectory = mBoxItem.Name.EndsWith("/");
                mFileItem.LastModified = mBoxItem.ModifiedAt.Value.Ticks;
                mFileItem.Size = (long)mBoxItem.Size;
                mFileItems.Add(mFileItem);
            }
            return mFileItems.ToArray();
        }
    }
}
