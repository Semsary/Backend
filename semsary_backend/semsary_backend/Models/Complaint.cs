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
        public CustomerService VerifiedBy {  get; set; }

        public string ComplaintReview { get; set; } 
        public string ComplaintDetails { get; set; }
        public Tenant SubmittedBy { get; set; }
        public DateTime SubmittingDate { get; set; }
        public DateTime ReviewDate { get; set; }
        
        public int RentalId { get; set; }  
        public Rental Rental { get; set; } 
    }
}
