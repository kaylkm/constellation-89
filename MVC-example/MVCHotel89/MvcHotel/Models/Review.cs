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
    }
}