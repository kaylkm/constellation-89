using System;

namespace MvcHotel.Data.Entities
{
    public class AdminReply
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string Text { get; set; } = "";
        public DateTimeOffset CreatedAt { get; set; }

        public Review Review { get; set; } = null!;
    }
}
