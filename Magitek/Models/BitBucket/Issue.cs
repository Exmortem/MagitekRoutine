using System;
using Newtonsoft.Json;

namespace Magitek.Models.BitBucket
{
    public class Issue
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("reported_by")]
        public ReportedBy ReportedBy { get; set; }

        [JsonProperty("utc_last_updated")]
        public string UtcLastUpdated { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("local_id")]
        public int LocalId { get; set; }

        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }

        [JsonProperty("utc_created_on")]
        public string UtcCreatedOn { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        [JsonProperty("is_spam")]
        public bool IsSpam { get; set; }
    }
}
