using Org.BouncyCastle.Bcpg.OpenPgp;
using semsary_backend.Enums;
using semsary_backend.Models;
using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class EditProfileDTO
    {

        public string ?Firstname { get; set; }
        public string ? Lastname { get; set; }
        //public Address? Address { get; set; }
        public Governorate? gover { get; set; }
        public string ?city { get; set; }
        public string ?street { get; set; }
        public IFormFile? ProfileImage { get; set; }
        [Range(50, 230, ErrorMessage = "high must be between 50 and 230.")]
        public int ?height { get; set; }
        [Range(30 , 500 , ErrorMessage ="wight must be between 30 and 500")]
        public float? weight { get; set; }
        public Gender ? gender { get; set; }
        [Range(16, 100, ErrorMessage = "Age must be between 16 and 120.")]
        public int ? age { get; set; }
        [Range(1, 20, ErrorMessage = "Number of people must be between 1 and 20.")]
        public int? NumberOfPeople { get; set; }
        public RentalType2? FavouriteRentalType { get; set; }
        public bool? IsSmoker { get; set; }
        public bool? NeedPublicService { get; set; }
        public bool? NeedPublicTransportation { get; set; }
        public bool? NeedNearUniversity { get; set; }
        public bool? NeedNearVitalPlaces { get; set; }

    }
}
