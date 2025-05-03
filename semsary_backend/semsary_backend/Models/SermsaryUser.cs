using semsary_backend.Enums;
using System.ComponentModel.DataAnnotations;
using semsary_backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace semsary_backend.Models
{
    public class SermsaryUser
    {
        public SermsaryUser()
        {
            Username = Ulid.NewUlid().ToString();
            
        }
        [Key]
        public  string Username { get; set; } 
        public string password { get; set; }
        public List<Email> Emails { get; set; }
        public string Firstname {  get; set; }
        public string Lastname { get; set; }
        public Address Address { get; set; }
        public UserType UserType { get; set; }
        [InverseProperty(nameof(Message.sender))]
        public ICollection<Message>? SentMessages { get; set; }
        [InverseProperty(nameof(Message.reciever))]
        public ICollection<Message>? ReceivedMessages { get; set; }



    }
}
