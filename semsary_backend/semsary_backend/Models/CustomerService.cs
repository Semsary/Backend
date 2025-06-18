namespace semsary_backend.Models
{
    public class CustomerService:semsary_backend.Models.SermsaryUser
    {
        public List<Complaint> Complaints { get; set; }
        public List<string>? DeviceTokens { get; set; }
        public List<BlockedId> BlockedIds { get; set; } = new List<BlockedId>();
    }
}
