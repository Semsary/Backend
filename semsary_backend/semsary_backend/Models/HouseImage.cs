namespace semsary_backend.Models
{
    public class HouseImage
    {
        public int HouseImageId { get; set; }
        public required string HouseInspectionId { get; set; }
        public required string ImageUrl { get; set; }
        public HouseInspection HouseInspection { get; set; }
    }
}
