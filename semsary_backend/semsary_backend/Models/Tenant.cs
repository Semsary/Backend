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
        public bool CompletedProfile { get; set; } = false;
        public int Balance { get; set; }
        public int height {  get; set; }
        public float weight {  get; set; }
        public int NumberOfPeople { get; set; }
        public RentalType2 FavouriteRentalType { get; set; }
        public Gender gender { get; set; }
        public int age { get; set; }
        public bool IsSmoker {  get; set; }
        public bool NeedPublicService {  get; set; }
        public bool NeedPublicTransportation {  get; set; }
        public bool NeedNearUniversity {  get; set; }
        public bool NeedNearVitalPlaces {  get; set; }
        public List<IdentityDocument> documents { get; set; }
        public List<string>? DeviceTokens { get; set; }


             
        public List<Rental> ?Rentals { get; set; }   
        public List<Rate> ?Rates { get; set; }
        public List<Comment>? Comments { get; set; }

        public List<Complaint>? Complaints { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public DateTime PremiumBegin { get; set; } = DateTime.MinValue;     
        public DateTime PremiumEnd { get; set; } = DateTime.MinValue;

    }
}
