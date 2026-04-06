using Microsoft.AspNetCore.Mvc;
using MvcHotel.Models;
using MvcHotel.ViewModels;

namespace MvcHotel.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = BuildHomeModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitReview(HomeIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var rebuilt = BuildHomeModel();
                rebuilt.ReviewForm = model.ReviewForm;
                return View("Index", rebuilt);
            }

            TempData["ReviewSuccess"] = "Дякуємо! Ваш відгук прийнято. Він з'явиться після модерації.";

            return RedirectToAction(nameof(Index));
        }

        private HomeIndexViewModel BuildHomeModel()
        {
            return new HomeIndexViewModel
            {
                Capsules = new List<Capsule>
                {
                    new Capsule
                    {
                        Id = "single",
                        Title = "Одномісна капсула",
                        Subtitle = "Загальний блок",
                        Description = "Ідеальний вибір для мандрівників — затишна капсула з усім необхідним для комфортного відпочинку.",
                        Price = 890,
                        ImageClass = "modal-img--standard",
                        Amenities = new List<string>
                        {
                            "🌡️ Клімат-контроль",
                            "💡 LED-освітлення",
                            "🔌 USB + розетка",
                            "📶 Wi-Fi 300 Мбіт/с"
                        }
                    },
                    new Capsule
                    {
                        Id = "singlew",
                        Title = "Одномісна капсула",
                        Subtitle = "Жіночий блок",
                        Description = "Розширена капсула з панорамним вікном і преміум-матрацом для тих, хто цінує особливий комфорт.",
                        Price = 1390,
                        ImageClass = "modal-img--premium",
                        Amenities = new List<string>
                        {
                            "🌡️ Клімат-контроль",
                            "🌈 RGB-підсвітка",
                            "🔌 USB-C + QI зарядка",
                            "📺 Smart TV 24\""
                        }
                    },
                    new Capsule
                    {
                        Id = "singlem",
                        Title = "Одномісна капсула",
                        Subtitle = "Чоловічий блок",
                        Description = "Найпростороніша капсула зі спеціальним зоряним куполом і сенсорним керуванням.",
                        Price = 2190,
                        ImageClass = "modal-img--lux",
                        Amenities = new List<string>
                        {
                            "🌡️ Multi-zone клімат",
                            "🌌 Зоряний купол",
                            "🔊 Аудіосистема Bose",
                            "🍳 Мінікухня"
                        }
                    },
                    new Capsule
                    {
                        Id = "double",
                        Title = "Двомісна капсула",
                        Subtitle = "Загальний блок",
                        Description = "Простора капсула для двох з роздільними ліжками та спільною зоною відпочинку.",
                        Price = 2190,
                        ImageClass = "modal-img--double",
                        Amenities = new List<string>
                        {
                            "🌡️ Клімат-контроль",
                            "💡 LED-освітлення",
                            "🔌 USB + розетка",
                            "📶 Wi-Fi 300 Мбіт/с"
                        }
                    }
                },
                Reviews = new List<Review>
                {
                    new Review
                    {
                        Author = "Олена Ковальчук",
                        CityAndDate = "Київ · Березень 2025",
                        Text = "Неймовірне місце! Відчуття ніби справді перебуваєш у космосі.",
                        Rating = 5,
                        AvatarLetter = "О"
                    },
                    new Review
                    {
                        Author = "Максим Бондаренко",
                        CityAndDate = "Львів · Лютий 2025",
                        Text = "Зупинявся на 3 ночі в капсулі Premium. Це просто вау!",
                        Rating = 5,
                        AvatarLetter = "М",
                        IsHighlighted = true
                    },
                    new Review
                    {
                        Author = "Соломія Іваненко",
                        CityAndDate = "Харків · Січень 2025",
                        Text = "Персонал дуже привітний, а сам готель — як щось з майбутнього.",
                        Rating = 5,
                        AvatarLetter = "С"
                    }
                }
            };
        }
    }
}