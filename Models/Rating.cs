using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    [Table("ratings")]
    public class Rating
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("review_id")]
        public int ReviewId { get; set; }

        [Column("general_impression")]
        public short? GeneralImpression { get; set; }

        [Column("cleanliness")]
        public short? Cleanliness { get; set; }

        [Column("staff")]
        public short? Staff { get; set; }

        [Column("price_quality")]
        public short? PriceQuality { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }
    }
}