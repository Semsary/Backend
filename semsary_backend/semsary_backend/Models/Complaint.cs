using semsary_backend.Enums;

namespace semsary_backend.Models
{
    public class Complaint
    {
        public int ComplaintId { get; set; }
        public ComplainStatus status { get; set; }
        public CustomerService VerifiedBy {  get; set; }
        public Tenant SubmittedBy { get; set; }

        public required DateTime ComplaintDate { get; set; }
        public required string ComplaintDetails { get; set; }

        public required int RentalId { get; set; }  
        public Rental Rental { get; set; } 
    }
}
