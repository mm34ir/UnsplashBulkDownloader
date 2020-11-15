using System.Text.Json;
using System.Text.Json.Serialization;

namespace Extensions {

    public static class Extensions {
        /// <summary>
        /// Print any object using the system json
        /// NOTE: this might get caught in infinite loops (look how to fix...)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pretty">print multi line json</param>
        /// <returns></returns>
        public static string ToJson(this object self, bool pretty = false) {
            return JsonSerializer.Serialize(self,
                                            new JsonSerializerOptions { WriteIndented = pretty });
        }
    }
}