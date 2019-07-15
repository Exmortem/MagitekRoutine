using Newtonsoft.Json;

namespace Magitek.Models.BitBucket
{
    public class Metadata
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("milestone")]
        public string Milestone { get; set; }
    }
}
