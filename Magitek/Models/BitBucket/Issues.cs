using System.Collections.Generic;
using Newtonsoft.Json;

namespace Magitek.Models.BitBucket
{
    public class Issues
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("search")]
        public object Search { get; set; }

        [JsonProperty("issues")]
        public IList<Issue> List { get; set; }
    }
}
