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
        public string VerifiedBy {  get; set; }   // customer service
        public string ComplaintReview { get; set; }   // customer service
        public string ComplaintDetails { get; set; }   // tenant
        public string SubmittedBy { get; set; }      // tenant
        public DateTime SubmittingDate { get; set; }   // tenant
        public DateTime ReviewDate { get; set; }    // customer service
        public int RentalId { get; set; }        // tenant
        

        public Rental Rental { get; set; } 
        public CustomerService CustomerService { get; set; }
        public Tenant Tenant { get; set; }
    }
}
