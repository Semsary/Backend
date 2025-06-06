using semsary_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class RentalDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public RentalType RentalType { get; set; }
        [Required]
        public string HouseId { get; set; }
        [Required]
        public DateTime StartArrivalDate { get; set; }
        [Required]
        public DateTime EndArrivalDate { get; set; }
        [Required]
        public List<string> RentalUnitIds { get; set; }
    }
}
