using System;

namespace MvcHotel.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Text { get; set; } = "";
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? EditedAt { get; set; }

        public User Author { get; set; } = null!;
        public Rating? Rating { get; set; }
        public AdminReply? AdminReply { get; set; }
    }
}
