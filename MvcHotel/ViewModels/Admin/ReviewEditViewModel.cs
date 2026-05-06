using System.ComponentModel.DataAnnotations;

namespace MvcHotel.ViewModels.Admin
{
    public class ReviewEditViewModel
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = "";
        public string ReviewText { get; set; } = "";
        public short? GeneralImpression { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public string? AdminReplyText { get; set; }
    }
}
