﻿using semsary_backend.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class HouseInspection
    {
        public HouseInspection()
        {
            inspectionStatus = InspectionStatus.Bending;
            HouseImages = new List<string>();
            HouseInspectionId=Ulid.NewUlid().ToString();

        } // unique random value
        public string ?longitude { get; set; }
        public string ?latitude { get; set; }
        public int FloorNumber {  get; set; }
        public int NumberOfAirConditionnar {  get; set; }
        public int NumberOfPathRooms {  get; set; }
        public int NumberOfBedRooms {  get; set; }
        public int NumberOfBeds {  get; set; }
        public int NumberOfBalacons {  get; set; }
        public int NumberOfTables {  get; set; }
        public int NumberOfChairs {  get; set; }
        public HouseFeature HouseFeature { get; set; }

        public int price {  get; set; }


        public string HouseInspectionId { get; set; }
        public DateTime? InspectionDate { get; set; }
        public DateTime? InspectionRequestDate { get; set; }

        
        public string? InspectorId { get; set; }     //customer service
        public string HouseId { get; set; }
        
        public InspectionStatus inspectionStatus { get; set; }
        public List<string> HouseImages { get; set; }
        [JsonIgnore]
        public House House { get; set; }
        [JsonIgnore]
        [ForeignKey("InspectorId")]
        public CustomerService Inspector { get; set; } //customer service
        public string? InspectionReport { get; set; } //json file
        

    }
}
