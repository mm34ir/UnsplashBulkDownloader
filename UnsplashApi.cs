using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace UnsplashBulkDownload {
    public class UnsplashApi {
        private static Uri BaseUri = new("https://api.unsplash.com/");
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

        public static async Task<IEnumerable<string>>
        GetCollectionPhotos(string id, string orientation = Orientation.Landscape) {
            var uri = new UriBuilder(BaseUri.ToString() + $"/collections/{id}/photos");
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["orientation"] = orientation;
            query["per_page"] = "100";
            uri.Query = query.ToString();
            var photos = await Http.GetFromJsonAsync<IEnumerable<CollectionResponsePhoto>>(uri.Uri) ?? throw new Exception("Failed to deserialize photos response");
            return photos.Select(photo => photo.Url);
        }
    }

    public class Orientation {
        public const string Landscape = "landscape";
    }

    public record CollectionResponsePhoto(
        Dictionary<string, string> Urls,
        string Description,
        string Id) {
        public string Url { get => Urls["regular"]; }
    }
}