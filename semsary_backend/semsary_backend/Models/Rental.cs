using semsary_backend.Enums;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Tenant Tenant { get; set; }
        [JsonIgnore]
        public House House { get; set; }
        public List<RentalUnit> RentalUnit { get; set; } 


    }
}