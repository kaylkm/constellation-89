using System.ComponentModel.DataAnnotations;

namespace MvcHotel.ViewModels.Admin
{
    public class UserEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [MaxLength(255, ErrorMessage = "Ім'я занадто довге")]
        public string Name { get; set; } = "";
    }
}
