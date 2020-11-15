using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Extensions;

namespace UnsplashBulkDownload {
    public class UnsplashApi {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
            PropertyNameCaseInsensitive = true
        };
        private static readonly Uri BaseUri = new("https://api.unsplash.com/");
        private static HttpClient? _Http;
        public static HttpClient Http {
            get {
                if (_Http is not null) {
                    return _Http;
                }
                _Http = new HttpClient();
                _Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Client-ID", Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_KEY")
                    );
                _Http.BaseAddress = BaseUri;
                return _Http;
            }
        }
        private static Uri BuildUri(string path, Dictionary<string, string>? query = null) {
            var uri = new UriBuilder(BaseUri.ToString() + path);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            queryParams["client_id"] = Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_KEY");
            if (query is not null) {
                foreach (var (key, value) in query) {
                    queryParams[key] = value;
                }
            }
            uri.Query = queryParams.ToString();
            Console.WriteLine($"Uri: {uri.Uri}");
            return uri.Uri;
        }

        public class CollectionDetails {
            public string? Id { get; set; }
            public string? Title { get; set; }
            [JsonPropertyName("total_photos")]
            public int? TotalPhotos { get; set; }
        }
        public static async Task<CollectionDetails> GetCollection(string id) {
            var uri = BuildUri($"/collections/{id}");
            var details = await Http.GetFromJsonAsync<CollectionDetails>(uri, JsonOptions);
            return details!;
        }

        public static async Task<IEnumerable<string>>
        GetCollectionPhotos(string id, string orientation = Orientation.Landscape) {
            var details = await GetCollection(id);
            Console.WriteLine(details.ToJson(pretty: true));
            var remainingPhotos = details.TotalPhotos!;

            var photos = new List<CollectionResponsePhoto>();
            var page = 1;
            while (remainingPhotos > 0) {
                var uri = BuildUri($"/collections/{id}/photos", new() {
                    { "orientation", orientation },
                    { "page", page.ToString() },
                });
                var photoBatch = await Http.GetFromJsonAsync<IEnumerable<CollectionResponsePhoto>>(uri, JsonOptions);
                photoBatch = photoBatch!.ToList();
                photos.AddRange(photoBatch!);
                remainingPhotos -= photoBatch.Count();
                page += 1;
                if (!photoBatch.Any()) {
                    break;
                }
            }
            return photos.Select(photo => photo.Url);
        }

        public static async Task DownloadPhoto(string url, string OutputDirectory = "./") {
            var uri = new Uri(url);
            var filename = OutputDirectory + url.Split("?").First().Split("/").Last();
            if (filename.ToLower().EndsWith(".jpg") is not true) {
                filename += ".jpg";
            }
            var response = await Http.GetAsync(uri);
            using var outFile = new FileStream(filename, FileMode.Create);
            await response.Content.CopyToAsync(outFile);
        }
    }

    public class Orientation {
        public const string Landscape = "landscape";
    }

    public record CollectionResponsePhoto(
        Dictionary<string, string> Urls,
        string Description,
        string Id) {
        public string Url { get => Urls["full"]; }
    }
}