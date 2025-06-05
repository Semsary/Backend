using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class RentalUnit
    {
        public string RentalUnitId { get; set; }
        public int MonthlyCost { get; set; }
        public int DailyCost { get; set; }
        public string AdvertisementId { get; set; }
        public List<Rental> Rentals { get; set; } = new List<Rental>();
        [JsonIgnore]
        public Advertisement Advertisement { get; set; } 
    }
}