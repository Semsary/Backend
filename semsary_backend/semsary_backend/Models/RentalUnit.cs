namespace semsary_backend.Models
{
    public class RentalUnit
    {
        public required string RentalUnitId { get; set; }
        public int MonthlyCost { get; set; }
        public int DailyCost { get; set; }
        public required string AdvertisementId { get; set; }
        public List<Rental> Rentals { get; set; } = new List<Rental>();
        public Advertisement Advertisement { get; set; } 
    }
}