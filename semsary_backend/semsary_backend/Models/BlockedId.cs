using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace semsary_backend.Models
{
    public class BlockedId
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Landlord))]
        public string SocialId { get; set; }
        public string Reason { get; set; }
        public DateTime BlockedDate { get; set; }

        [ForeignKey(nameof(customerService))]
        public string BlockedBy { get; set; }
        public Landlord Landlord { get; set; }
        public CustomerService customerService { get; set; }
    }
}
