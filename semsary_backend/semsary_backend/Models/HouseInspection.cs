using semsary_backend.Enums;

namespace semsary_backend.Models
{
    public class HouseInspection
    {
        public HouseInspection()
        {
            inspectionStatus = InspectionStatus.Bending;
        }
        public int FloorNumber {  get; set; }
        public int NumberOfAirConditionnar {  get; set; }
        public int NumberOfPathRooms {  get; set; }
        public int NumberOfBedRooms {  get; set; }
        public int NumberOfBeds {  get; set; }
        public int NumberOfBalacons {  get; set; }
        public int NumberOfTables {  get; set; }
        public int NumberOfChairs {  get; set; }
        public bool HaveNearHospital {  get; set; }
        public bool HaveNearGym {  get; set; }
        public bool HaveNearPlayGround {  get; set; }
        public bool HaveNearSchool {  get; set; }
        public bool HaveNearUniversity {  get; set; }
        public bool HaveNearSupermarket {  get; set; }
        public bool HaveNearRestaurant {  get; set; }
        public bool HaveNearBusStation {  get; set; }
        public bool HaveNearBank {  get; set; }
        public bool HaveWiFi {  get; set; }
        public bool HaveTV {  get; set; }
        public bool Havekitchen {  get; set; }
        public bool HaveElevator {  get; set; }
        public bool HaveWashingMachine {  get; set; }
        public bool HaveCooker {  get; set; }
        public bool HaveFridge {  get; set; }
        public bool HaveHeater {  get; set; }
        public bool HaveInternet {  get; set; }
        public bool HaveSalon {  get; set; }
        public bool DiningRoom {  get; set; }
        

        public int price {  get; set; }


        public string HouseInspectionId { get; set; }
        public DateTime InspectionDate { get; set; }
        public string InspectorId { get; set; }     //customer service
        public string HouseId { get; set; }
        
        public InspectionStatus inspectionStatus { get; set; }
        public List<HouseImage> HouseImages { get; set; }
        public House House { get; set; }

    }
}
