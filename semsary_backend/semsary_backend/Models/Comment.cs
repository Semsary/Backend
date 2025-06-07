using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string HouseId { get; set; }
        public string TenantUsername { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentDetails { get; set; }
        [JsonIgnore]
        public Tenant Tenant { get; set; }
        [JsonIgnore]
        public House House { get; set; }
    }
}
