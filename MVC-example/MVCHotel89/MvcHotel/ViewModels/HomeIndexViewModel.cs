using MvcHotel.Models;

namespace MvcHotel.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Capsule> Capsules { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public ReviewFormViewModel ReviewForm { get; set; } = new();
    }
}