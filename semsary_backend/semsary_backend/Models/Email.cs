using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace semsary_backend.Models
{
    public enum OTPType
    {
        Verification,
        PasswordReset

    }
    
    public class Email
    {
        public Email() 
        {
            otp = "000000";
            OtpExpiry = DateTime.MinValue;
            NumberofMismatch = 0;
            otpType= OTPType.Verification;

        }
        [Key]
        public string email {  get; set; }
        public bool IsVerified {  get; set; }=false;
        public string ownerUsername {  get; set; }
        public semsary_backend.Models.SermsaryUser owner { get; set; }
        public string otp { get; set; }
        public DateTime? OtpExpiry { get; set; } 

        public int NumberofMismatch { get; set; } 
        public OTPType otpType { get; set; } 


    }
}
