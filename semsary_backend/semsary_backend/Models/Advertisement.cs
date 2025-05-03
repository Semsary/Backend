namespace semsary_backend.Models
{
    public class Advertisement
    {
        public required string AdvertisementId { get; set; }
        public required string HouseId { get; set; }  
        public required DateTime PublishDate { get; set; }
        public List<RentalUnit> RentalUnits { get; set; } = new List<RentalUnit>();
        public House House { get; set; } 

    }
}