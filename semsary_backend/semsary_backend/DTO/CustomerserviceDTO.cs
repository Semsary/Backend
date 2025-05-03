using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class CustomerserviceDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d\W]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
        public required string password { get; set; }
        

    }
}
