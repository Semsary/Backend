using semsary_backend.Enums;
using semsary_backend.Models;

namespace semsary_backend.DTO
{
    public class HouseInspectionDTO
    {
        public int FloorNumber { get; set; }
        public int NumberOfAirConditionnar { get; set; }
        public int NumberOfPathRooms { get; set; }
        public int NumberOfBedRooms { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfBalacons { get; set; }
        public int NumberOfTables { get; set; }
        public int NumberOfChairs { get; set; }
        public HouseFeature HouseFeature { get; set; }
        public List<IFormFile> HouseImages { get; set; }

    }
}
