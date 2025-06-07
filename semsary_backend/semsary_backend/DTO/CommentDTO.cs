using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class CommentDTO
    {
        [Required]
        public string CommentDetails { get; set; }
    }
}
