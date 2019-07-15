using Newtonsoft.Json;

namespace Magitek.Models.Authentication
{
    internal class ClientSession
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("product")]
        public int Product { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
