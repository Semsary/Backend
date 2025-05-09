using semsary_backend.Enums;
using semsary_backend.Models;

namespace semsary_backend.DTO
{
    public class ComplaintReviewDTO
    {
        public string VerifiedBy { get; set; }
        public string ComplaintReview { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
