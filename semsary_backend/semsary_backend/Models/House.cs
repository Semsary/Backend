namespace semsary_backend.Models
{
    public class House
    {
        public required string HouseId { get; set; }
        public required string LandlordUsername { get; set; }  
        public string Address { get; set; }
        public List<Rate> ?Rates { get; set; } 
        public List<HouseInspection> HouseInspections { get; set; }
        public List<Advertisement> Advertisements { get; set; } 
        public Landlord Landlord { get; set; }
    }
}
