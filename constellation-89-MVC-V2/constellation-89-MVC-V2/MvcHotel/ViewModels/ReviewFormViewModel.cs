using System.ComponentModel.DataAnnotations;

namespace MvcHotel.ViewModels
{
    public class ReviewFormViewModel
    {
        [Required]
        [Display(Name = "Ваше ім'я")]
        public string Name { get; set; } = "";

        [Required]
        [Display(Name = "Яку капсулу обирали?")]
        public string CapsuleType { get; set; } = "";

        [Range(1, 5)]
        [Display(Name = "Загальне враження")]
        public int Rating { get; set; }

        [Range(1, 5)]
        [Display(Name = "Чистота")]
        public int RatingClean { get; set; }

        [Range(1, 5)]
        [Display(Name = "Персонал")]
        public int RatingStaff { get; set; }

        [Range(1, 5)]
        [Display(Name = "Ціна / Якість")]
        public int RatingValue { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Ваш відгук")]
        public string Text { get; set; } = "";
    }
}
