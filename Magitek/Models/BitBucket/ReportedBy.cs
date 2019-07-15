using Newtonsoft.Json;

namespace Magitek.Models.BitBucket
{
    public class ReportedBy
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("is_staff")]
        public bool IsStaff { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        [JsonProperty("is_team")]
        public bool IsTeam { get; set; }
    }
}
