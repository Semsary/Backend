namespace semsary_backend.Models
{
    public class Landlord:UnverifiedUser
    {
        public Landlord()
        {
            isVerified = false;
            isBlocked = false;

        }
        public int Balance { get; set; } = 1000000000;
        public string ?ImageUrl { get; set; }
        public List<string> DeviceTokens { get; set; }
        public List<House> ?Houses { get; set; }
        public List<Message> ?SentMessages { get; set; }
        public List<Message>? ReceivedMessages { get; set; }
        

        public bool isVerified { get; set; } 
        public bool isBlocked { get; set; } 
        


    }
}
