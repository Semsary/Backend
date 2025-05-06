using semsary_backend.DTO;
using semsary_backend.Enums;
using semsary_backend.Models;
using System.Text.Json.Serialization;
namespace semsary_backend.Models
{
    public class IdentityDocument
    {
        public string OwnerUsername { get; set; }
        public List<string>ImageURLS { get; set; }
        public DateTime SubmitedAt { get; set; }
        public DateTime ReviewedAt { get; set; }
        public IdentityStatus Status { get; set; }
        public string ?ReviewerUsername { get; set; }
        public string ?Comment { get; set; }
        public string ?Id { get; set; }
        [JsonIgnore]
        public UnverifiedUser Owner { get; set; }
        [JsonIgnore]
        public CustomerService? reviewer { get; set; }
        public IdentityDocument()
            {
                ImageURLS = new List<string>();
                SubmitedAt = DateTime.UtcNow;
                ReviewedAt = DateTime.MinValue;
                Status = IdentityStatus.Pending;
                ReviewerUsername = null;
                Comment = null;
                Id = Ulid.NewUlid().ToString();
            }

    }
}
