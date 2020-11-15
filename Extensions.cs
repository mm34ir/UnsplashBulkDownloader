using System.Text.Json;
using System.Text.Json.Serialization;

namespace Extensions {

    public static class Extensions {
        /// <summary>
        /// Print any object using the system json
        /// NOTE: reference counting is disabled for now
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pretty">print multi line json</param>
        /// <param name="handleReferences">turn on reference handling (which will look ugly, but not error out)</param>
        /// <returns></returns>
        public static string ToJson(this object self, bool pretty = false, bool handleReferences = false) {
            var options = new JsonSerializerOptions { WriteIndented = pretty };
            if (handleReferences) {
                options.ReferenceHandler = ReferenceHandler.Preserve;
            }
            return JsonSerializer.Serialize(self, options);
        }
    }
}