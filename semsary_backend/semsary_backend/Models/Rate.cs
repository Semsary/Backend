namespace semsary_backend.Models
{
    public class Rate
    {
        public int RateId { get; set; }
        public required string HouseId { get; set; }
        public required string TenantUsername { get; set; }
        public required DateTime RateDate { get; set; }
        public required byte StarsNumber { get; set; }
        public string ?RateDetails { get; set; }
        
        public Tenant Tenant { get; set; }
        public House House { get; set; }    
    }
}
