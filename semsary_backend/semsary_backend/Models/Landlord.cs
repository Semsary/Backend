using System.ComponentModel.DataAnnotations;

namespace semsary_backend.Models
{
    public class Landlord:UnverifiedUser
    {
        public Landlord()
        {

        }
        public string? SocialId { get; set; }
        public List<string> ?DeviceTokens { get; set; }
        public List<House> ?Houses { get; set; }
        public BlockedId BlockedId { get; set; }
    }
}
