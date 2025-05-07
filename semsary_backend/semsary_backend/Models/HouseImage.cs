namespace semsary_backend.Models
{
    public class HouseImage
    {
        public int HouseImageId { get; set; }
        public string HouseInspectionId { get; set; }
        public string ImageUrl { get; set; }
        public HouseInspection? HouseInspection { get; set; }
    }
}
