using semsary_backend.Enums;
using semsary_backend.Models;

namespace semsary_backend.DTO
{
    public class ComplaintRequestDTO
    {
        public string SubmittedBy { get; set; }
        public  DateTime SubmittingDate { get; set; }
        public string ComplaintDetails { get; set; }
        public int RentalId { get; set; }

    }
}
