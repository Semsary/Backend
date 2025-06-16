using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace semsary_backend.Models
{
    public class Coupon
    {
        public Ulid Id { get; set; }
        public int Balance { get; set; }
        public string? CreatedBy { get; set; }
        public string? ExPosedPy { get; set; }
        public bool IsUsed { get; set; }

        public Coupon()
        {
            Id = Ulid.NewUlid();
            IsUsed = false;

        }

    }
}
