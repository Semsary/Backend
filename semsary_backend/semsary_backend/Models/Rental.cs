using semsary_backend.Enums;

namespace semsary_backend.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; } 
        public string RentalUnitId { get; set; } 
        public int RentalType { get; set; } 
        public string TenantUsername { get; set; }  
        public string HouseId { get; set; }
        public DateTime CreationDate { get; set; } 
        public DateTime InspectionDate { get; set; }
        public RentalStatus status { get; set; }

        public Complaint Complaint { get; set; } 
        public Tenant Tenant { get; set; }
        public House House { get; set; }
        public RentalUnit RentalUnit { get; set; } 


    }
}