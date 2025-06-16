using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class RentalPeriodDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime StartArrivalDate { get; set; }
        [Required]
        public DateTime EndArrivalDate { get; set; }
        [Required]
        public string AdvId { get; set; }
    }
}
