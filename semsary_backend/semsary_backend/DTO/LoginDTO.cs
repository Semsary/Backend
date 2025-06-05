using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage ="invalid email")]
        public required string Email {  get; set; }
        [MaxLength(100)]
        [MinLength(8)]
        public required string Password { get; set; }
        public string? deviceToken { get; set; }
    }
}
