using System.ComponentModel.DataAnnotations;
using semsary_backend.Models;
namespace semsary_backend.DTO
{
    public class SignUpLandlord
    {
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d\W]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
        public string Password { get; set; }
        public required string firstname {  get; set; }
        public  string lastname { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="invalid email")]
        public string Email { get; set; }

        
    }
}
