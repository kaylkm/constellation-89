namespace MvcHotel.Models
{
    public class Review
    {
        public string Author { get; set; } = "";
        public string CityAndDate { get; set; } = "";
        public string Text { get; set; } = "";
        public int Rating { get; set; }
        public string AvatarLetter { get; set; } = "";
        public bool IsHighlighted { get; set; }

        // Детальні оцінки
        public int? Cleanliness { get; set; }
        public int? Staff { get; set; }
        public int? PriceQuality { get; set; }

        // Відповідь готелю
        public string? AdminReply { get; set; }
    }
}
