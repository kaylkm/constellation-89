using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    [Table("admin_replies")]
    public class AdminReply
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("text")]
        public string Text { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }
    }
}