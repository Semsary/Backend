using semsary_backend.Enums;
using semsary_backend.Models;

namespace semsary_backend.DTO
{
    public class EditProfileDTO
    {

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Address Address { get; set; }
        public IFormFile? ProfileImage { get; set; }

        public int height { get; set; }
        public Gender gender { get; set; }
        public int age { get; set; }
        public bool IsSmoker { get; set; }
        public bool NeedPublicService { get; set; }
        public bool NeedPublicTransportation { get; set; }
        public bool NeedNearUniversity { get; set; }
        public bool NeedNearVitalPlaces { get; set; }

    }
}
