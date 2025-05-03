using System.ComponentModel.DataAnnotations.Schema;

namespace semsary_backend.Models
{
    
    public class Message
    {
        public int MessageId { get; set; }
        public required string SenderUsername { get; set; }
        public required string ReceiverUsername { get; set; }
        [ForeignKey(nameof(SenderUsername))]
        [InverseProperty(nameof(SermsaryUser.SentMessages))]
        public semsary_backend.Models.SermsaryUser sender {  get; set; }
        [ForeignKey(nameof(ReceiverUsername))]
        [InverseProperty(nameof (SermsaryUser.ReceivedMessages))]
        public semsary_backend.Models.SermsaryUser reciever {  get; set; }
        public DateTime SendDate { get; set; }
        public required string Content { get; set; }

    }
}