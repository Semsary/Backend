using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string SentTo { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public Tenant Tenant { get; set; }
        [JsonIgnore]
        public Landlord Landlord { get; set; }

    }
}
