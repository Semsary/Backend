using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class IdentityDTO
    {
        [Required]
        public IFormFile UserImage { get; set; }
        [Required]
        public IFormFile IdentityFront { get; set; }
        [Required]
        public IFormFile IdentityBack { get; set; }


    }
}
