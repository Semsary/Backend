using semsary_backend.DTO;

namespace semsary_backend.Models
{
    public class UnverifiedUser:SermsaryUser
    {
        public List<IdentityDocument>Identity { get; set; }
        public bool IsVerified { get; set; }
        public UnverifiedUser()
        {
            IsVerified = false;
            Identity= new List<IdentityDocument>();
        }



    }
}
