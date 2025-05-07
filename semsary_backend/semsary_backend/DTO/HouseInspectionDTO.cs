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

        public bool HaveNearHospital { get; set; }
        public bool HaveNearGym { get; set; }
        public bool HaveNearPlayGround { get; set; }
        public bool HaveNearSchool { get; set; }
        public bool HaveNearUniversity { get; set; }
        public bool HaveNearSupermarket { get; set; }
        public bool HaveNearRestaurant { get; set; }
        public bool HaveNearBusStation { get; set; }
        public bool HaveNearBank { get; set; }

        public bool HaveWiFi { get; set; }
        public bool HaveTV { get; set; }
        public bool Havekitchen { get; set; }
        public bool HaveElevator { get; set; }
        public bool HaveWashingMachine { get; set; }
        public bool HaveCooker { get; set; }
        public bool HaveFridge { get; set; }
        public bool HaveHeater { get; set; }
        public bool HaveInternet { get; set; }
        public bool HaveSalon { get; set; }
        public bool DiningRoom { get; set; }

        public List<HouseImage> HouseImages { get; set; }

    }
}
