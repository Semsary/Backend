namespace semsary_backend.Models
{
    public class Landlord:UnverifiedUser
    {
        public Landlord()
        {

        }
        public string ?ImageUrl { get; set; }
        public List<string> DeviceTokens { get; set; }
        public List<House> ?Houses { get; set; }
        

        


    }
}
