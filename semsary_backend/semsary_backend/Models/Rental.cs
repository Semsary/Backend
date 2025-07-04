﻿using semsary_backend.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace semsary_backend.Models
{
    public class Rental
    {
        public Rental()
        {
            NumOfComments = 0;
        }

        public int RentalId { get; set; }
        public int WarrantyMoney { get; set; }
        public int NumOfComments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public RentalType RentalType { get; set; } 
        public string TenantUsername { get; set; }  
        public string HouseId { get; set; }
        public DateTime ResponseDate { get; set; }
        public DateTime CreationDate { get; set; } 
        public DateTime StartArrivalDate { get; set; }
        public DateTime EndArrivalDate { get; set; }
        public RentalStatus status { get; set; }
        public List<string> RentalUnitIds { get; set; } 

        public Complaint Complaint { get; set; }
        [NotMapped]
        public static readonly double OurPercentage = 0.05; 
        [JsonIgnore]
        public Tenant Tenant { get; set; }
        [JsonIgnore]
        public House House { get; set; }
        public List<RentalUnit> RentalUnit { get; set; } 


    }
}