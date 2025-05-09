using semsary_backend.Enums;

namespace semsary_backend.Models
{
    public class House
    {
        public required string HouseId { get; set; }
        public string LandlordUsername { get; set; }  
        public Address Address { get; set; }
        public List<Rate> ?Rates { get; set; } 
        public List<HouseInspection> HouseInspections { get; set; }
        public List<Advertisement> Advertisements { get; set; } 
        public Landlord Landlord { get; set; }
        public Landlord owner { get; set; }
    }
}
