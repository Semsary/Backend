namespace semsary_backend.Models
{
    public class CustomerService:semsary_backend.Models.SermsaryUser
    {
        public List<Complaint> Complaints { get; set; }

    }
}
