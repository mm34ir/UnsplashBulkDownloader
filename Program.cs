using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using UnsplashBulkDownload;

static string GetOutputFolder() {
    var folder = Environment.GetEnvironmentVariable("BACKGROUND_FOLDER") ?? "./Photos/";
    if (folder.EndsWith("/") is not true) {
        folder += "/";
    }
    return folder;
}

string OutputFolder = GetOutputFolder();
bool CreateFolder = (Environment.GetEnvironmentVariable("CREATE_FOLDER") ?? "true") == "true";
bool ClearFolder = (Environment.GetEnvironmentVariable("CLEAR_FOLDER") ?? "true") == "true";

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
