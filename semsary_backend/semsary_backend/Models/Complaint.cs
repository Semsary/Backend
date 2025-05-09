using semsary_backend.Enums;

namespace semsary_backend.Models
{
    public class Complaint
    {
        public Complaint()
        {
            status = ComplainStatus.Bending;
        }
        public int ComplaintId { get; set; }
        public ComplainStatus status { get; set; }
        public string VerifiedBy {  get; set; }
        public string ComplaintReview { get; set; } 
        public string ComplaintDetails { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittingDate { get; set; }
        public DateTime ReviewDate { get; set; }
        
        public int RentalId { get; set; }  
        public Rental Rental { get; set; } 
        public CustomerService CustomerService { get; set; }
        public Tenant Tenant { get; set; }
    }
}
