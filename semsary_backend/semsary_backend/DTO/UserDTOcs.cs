using semsary_backend.Enums;
using semsary_backend.Models;

namespace semsary_backend.DTO
{
    public class UserDTOcs
    {
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public List<Email>emails { get; set; }
        public Address address { get; set; }
        public UserType userType { get; set; }

    }
}
