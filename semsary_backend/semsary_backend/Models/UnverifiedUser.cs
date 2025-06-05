using Org.BouncyCastle.Crypto;
using semsary_backend.DTO;

namespace semsary_backend.Models
{
    public class UnverifiedUser:SermsaryUser
    {
        public List<IdentityDocument>Identity { get; set; }
        public bool IsVerified { get; set; }
        public bool IsBlocked {  get; set; }
        public int Balance {  get; set; }
        public UnverifiedUser()
        {
            Balance = 10000000;
            IsVerified = false;
            IsBlocked = false;
            Identity = new List<IdentityDocument>();
        }



    }
}
