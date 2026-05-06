namespace MvcHotel.Data.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public short? GeneralImpression { get; set; }
        public short? Cleanliness { get; set; }
        public short? Staff { get; set; }
        public short? PriceQuality { get; set; }

        public Review Review { get; set; } = null!;
    }
}
