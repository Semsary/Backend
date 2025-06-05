using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class Rate
    {
        public int RateId { get; set; }
        public string HouseId { get; set; }
        public string TenantUsername { get; set; }
        public DateTime RateDate { get; set; }
        public byte StarsNumber { get; set; }
        public string ?RateDetails { get; set; }
        [JsonIgnore]
        public Tenant Tenant { get; set; }
        [JsonIgnore]
        public House House { get; set; }    
    }
}
