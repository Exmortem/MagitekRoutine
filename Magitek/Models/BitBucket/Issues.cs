using Newtonsoft.Json;
using System.Collections.Generic;

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
