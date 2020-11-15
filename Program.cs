using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions;

namespace UnsplashBulkDownload {
    class Program {
        private static string OutputFolder {
            get {
                var folder = Environment.GetEnvironmentVariable("BACKGROUND_FOLDER") ?? "./Photos/";
                if (folder.EndsWith("/") is not true) {
                    folder += "/";
                }
                return folder;
            }
        }
        private static bool CreateFolder => (Environment.GetEnvironmentVariable("CREATE_FOLDER") ?? "true") == "true";
        private static bool ClearFolder => (Environment.GetEnvironmentVariable("CLEAR_FOLDER") ?? "true") == "true";
        static async Task Main(string[] args) {
            if (!Directory.Exists(OutputFolder) && CreateFolder is true) {
                Directory.CreateDirectory(OutputFolder);
            }
            if (ClearFolder is true) {
                foreach (var file in Directory.EnumerateFiles(OutputFolder)) {
                    File.Delete(file);
                }
            }

            var api = new UnsplashApi();
            var photos = await UnsplashApi.GetCollectionPhotos("319663");
            Console.WriteLine(photos.ToJson(pretty: true));
            var downloads = photos.Select(photo => UnsplashApi.DownloadPhoto(photo, OutputFolder));
            await Task.WhenAll(downloads);
        }
    }
}
