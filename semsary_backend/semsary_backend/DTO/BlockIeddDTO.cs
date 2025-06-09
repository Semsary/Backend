using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class BlockIeddDTO
    {
        [Required]
        public string Reason { get; set; }
    }
}
