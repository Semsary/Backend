using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace semsary_backend.Models
{
    public class BlockedId
    {
        [Key]
        public int Id { get; set; }

        public string SocialId { get; set; }
        public string Reason { get; set; }
        public DateTime BlockedDate { get; set; }

        // Link to CustomerService (many-to-one)
        [ForeignKey(nameof(customerService))]
        public string BlockedBy { get; set; }
        public CustomerService customerService { get; set; }

        // Link to Landlord (one-to-one)
        public string LandlordId { get; set; }

        [ForeignKey(nameof(LandlordId))]
        public Landlord Landlord { get; set; }
    }
}
