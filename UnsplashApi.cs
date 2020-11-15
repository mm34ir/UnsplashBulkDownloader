using System.Net.Http;

namespace UnsplashBulkDownload
{
    public class UnsplashApi {
        public static HttpClient Http { get; } = new HttpClient();
    }
}