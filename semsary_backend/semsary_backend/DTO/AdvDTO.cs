using semsary_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class AdvDTO
    {
        public AdvDTO() { }
        [Required(ErrorMessage = "house id is required")]
        public string HouseId {  get; set; }
        [Required(ErrorMessage = "rental type is required")]
        public RentalType RentalType { get; set; }
        public int MonthlyCost {  get; set; }
        public int DailyCost {  get; set; }

    }
}
