using semsary_backend.Enums;
using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class House
    {
        public required string HouseId { get; set; }
        public string LandlordUsername { get; set; }  
        public Governorate governorate { get; set; }
        public string? city { get; set; }
        public string? street { get; set; }

        public List<Rate> ?Rates { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<HouseInspection> HouseInspections { get; set; }
        public List<Advertisement> Advertisements { get; set; }
        [JsonIgnore]
        public Landlord owner { get; set; }
        public List<Rental> Rentals { get; set; }
    }
}
