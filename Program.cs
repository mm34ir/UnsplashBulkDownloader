using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;

using Extensions;
using UnsplashBulkDownload;

string OutputFolder = Environment.GetEnvironmentVariable("BACKGROUND_FOLDER") ?? "./Photos/";
bool CreateFolder = (Environment.GetEnvironmentVariable("CREATE_FOLDER") ?? "true") == "true";
bool ClearFolder = (Environment.GetEnvironmentVariable("CLEAR_FOLDER") ?? "false") == "true";

var options = Parser.Default.ParseArguments<Options>(args);
var result = options.WithParsed(options => {
    Console.WriteLine(options.ToJson(pretty: true));
    if (options.OutputDirectory is not null) {
        OutputFolder = options.OutputDirectory;
    }
    if (options.CleanDirectory is bool clean) {
        ClearFolder = clean;
    }
});

OutputFolder = PathHelper.ToFolder(OutputFolder);
if (!Directory.Exists(OutputFolder) && CreateFolder is true) {
    Directory.CreateDirectory(OutputFolder);
}
if (ClearFolder is true) {
    foreach (var file in Directory.EnumerateFiles(OutputFolder)) {
        if (file.EndsWith(".jpg")) {
            File.Delete(file);
        }
    }
}

var photos = await UnsplashApi.GetCollectionPhotos("319663");
Console.WriteLine(photos.ToJson(pretty: true));
var downloads = photos.Select(photo => UnsplashApi.DownloadPhoto(photo, OutputFolder));
await Task.WhenAll(downloads);

class PathHelper {
    public static string ToFolder(string folder) {
        if (folder.EndsWith("/") is not true) {
            folder += "/";
        }
        return folder;
    }
}

class Options {
    [Option('o', "output", Required = false, HelpText = "Folder to download files to")]
    public string? OutputDirectory { get; set; }

    [Option('c', "create", Required = false, HelpText = "Folder to download files to", Default = false)]
    public bool CreateDirectory { get; set; }

    [Option("clean", Required = false, HelpText = "Delete all jpg files in the existing folder", Default = false)]
    public bool CleanDirectory { get; set; }
}
