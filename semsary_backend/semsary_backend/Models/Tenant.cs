using semsary_backend.Enums;

namespace semsary_backend.Models
{
    public enum Gender
    {
        male,
        female,
    }
    
    public class Tenant:UnverifiedUser
    {
        public int Balance { get; set; }
        public string ?ImageUrl { get; set;}
        public int height {  get; set; }
        public Gender gender { get; set; }
        public Governorate governorate { get; set; }
        public List<IdentityDocument> documents { get; set; }


        public int age { get; set; }
        public bool IsSmoker {  get; set; }
        public bool NeedPublicService {  get; set; }
        public bool NeedPublicTransportation {  get; set; }
        public bool NeedNearUniversity {  get; set; }
        public bool NeedNearVitalPlaces {  get; set; }
             
        public List<Rental> ?Rentals { get; set; }   
        public List<Rate> ?Rates { get; set; }     
        public List<Message> ?SentMessages { get; set; }
        public List<Message> ?ReceivedMessages { get; set; }
        




    }
}
