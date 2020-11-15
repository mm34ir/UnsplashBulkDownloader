using System;
using System.Threading.Tasks;
using Extensions;

namespace UnsplashBulkDownload {
    class Program {
        static async Task Main(string[] args) {
            Console.WriteLine("Hello World!");
            var api = new UnsplashApi();
            var results = await UnsplashApi.GetCollectionPhotos("319663");
            Console.WriteLine(results.ToJson(pretty: true));
        }
    }
}
