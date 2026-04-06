namespace MvcHotel.Models
{
    public class Capsule
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string ImageClass { get; set; } = "";
        public string Badge { get; set; } = "";
        public string BadgeClass { get; set; } = "";
        public List<string> Amenities { get; set; } = new();
    }
}