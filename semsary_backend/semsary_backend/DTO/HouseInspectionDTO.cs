using semsary_backend.Enums;
using semsary_backend.Models;
using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class HouseInspectionDTO
    {
        [Required]
        public required string longitude { get; set; }
        [Required]
        public required string latitude { get; set; }
        [Required]
        public int FloorNumber { get; set; }
        [Required]
        public int NumberOfAirConditionnar { get; set; }
        [Required]
        public int NumberOfPathRooms { get; set; }
        [Required]
        public int NumberOfBedRooms { get; set; }
        [Required]
        public int NumberOfBeds { get; set; }
        [Required]
        public int NumberOfBalacons { get; set; }
        [Required]
        public int NumberOfTables { get; set; }
        [Required]
        public int NumberOfChairs { get; set; }
        [Required]
        public List<HouseFeature> HouseFeatures { get; set; }
        [Required]
        public List<IFormFile> HouseImages { get; set; }
    }
}
