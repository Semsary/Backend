using semsary_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class FilterDTO
    {
        public Governorate? governorate { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? MinMonthlyCost { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? MaxMonthlyCost { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? MinDailyCost { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? MaxDailyCost { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? FloorNumber { get; set; }
        [Range(1, 15, ErrorMessage = "This value must be greater than 0")]
        public int? NumOfBedrooms { get; set; }
        [Range(1, 5, ErrorMessage = "This value must be greater than 0")]
        public int? NumOfBathrooms { get; set; }
        public RentalType? rentalType { get; set; }
    }
}
