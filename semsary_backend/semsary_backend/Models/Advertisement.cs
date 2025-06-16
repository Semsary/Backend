using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class Advertisement
    {
        public Advertisement()
        {
        }
        public required string AdvertisementId { get; set; }
        public string HouseName { get; set; }
        public string houseDescription { get; set; }
        public Enums.RentalType rentalType { get; set; }
        public required string HouseId { get; set; }  
        public required DateTime PublishDate { get; set; }
        public List<RentalUnit> RentalUnits { get; set; } = new List<RentalUnit>();
        [JsonIgnore]
        public House House { get; set; } 

    }
}