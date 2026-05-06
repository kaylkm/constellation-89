using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;
using MvcHotel.Models;
using MvcHotel.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcHotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly HotelDbContext _context;

        public HomeController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildHomeModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(HomeIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var rebuilt = await BuildHomeModelAsync();
                rebuilt.ReviewForm = model.ReviewForm;
                return View("Index", rebuilt);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == model.ReviewForm.Name);
            if (user == null)
            {
                user = new User { Name = model.ReviewForm.Name };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var review = new Data.Entities.Review
            {
                AuthorId = user.Id,
                Text = model.ReviewForm.Text,
                CreatedAt = System.DateTimeOffset.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var rating = new Rating
            {
                ReviewId = review.Id,
                GeneralImpression = model.ReviewForm.Rating > 0 ? (short)model.ReviewForm.Rating : null,
                Cleanliness = model.ReviewForm.RatingClean > 0 ? (short)model.ReviewForm.RatingClean : null,
                Staff = model.ReviewForm.RatingStaff > 0 ? (short)model.ReviewForm.RatingStaff : null,
                PriceQuality = model.ReviewForm.RatingValue > 0 ? (short)model.ReviewForm.RatingValue : null
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            TempData["ReviewSuccess"] = "Дякуємо! Ваш відгук прийнято. Він з'явиться після модерації.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<HomeIndexViewModel> BuildHomeModelAsync()
        {
            var prices = await _context.CapsulePrices
                .ToDictionaryAsync(p => p.Slug, p => p.Price);

            int PriceOf(string slug, int fallback) =>
                prices.TryGetValue(slug, out var p) ? p : fallback;

            var dbReviews = await _context.Reviews
                .Include(r => r.Author)
                .Include(r => r.Rating)
                .Include(r => r.AdminReply)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync();

            var viewReviews = dbReviews.Select(r => new Models.Review
            {
                Author = r.Author.Name,
                CityAndDate = "Онлайн · " + r.CreatedAt.ToString("MMMM yyyy"),
                Text = r.Text,
                Rating = r.Rating?.GeneralImpression ?? 5,
                AvatarLetter = string.IsNullOrEmpty(r.Author.Name) ? "U" : r.Author.Name.Substring(0, 1).ToUpper(),
                IsHighlighted = r.Rating?.GeneralImpression == 5,
                Cleanliness = r.Rating?.Cleanliness.HasValue == true ? (int?)r.Rating.Cleanliness.Value : null,
                Staff = r.Rating?.Staff.HasValue == true ? (int?)r.Rating.Staff.Value : null,
                PriceQuality = r.Rating?.PriceQuality.HasValue == true ? (int?)r.Rating.PriceQuality.Value : null,
                AdminReply = r.AdminReply?.Text
            }).ToList();

            if (!viewReviews.Any())
            {
                viewReviews = new List<Models.Review>
                {
                    new Models.Review
                    {
                        Author = "Олена Ковальчук",
                        CityAndDate = "Київ · Березень 2025",
                        Text = "Неймовірне місце! Відчуття ніби справді перебуваєш у космосі.",
                        Rating = 5,
                        AvatarLetter = "О"
                    },
                    new Models.Review
                    {
                        Author = "Максим Бондаренко",
                        CityAndDate = "Львів · Лютий 2025",
                        Text = "Зупинявся на 3 ночі в капсулі Premium. Це просто вау!",
                        Rating = 5,
                        AvatarLetter = "М",
                        IsHighlighted = true
                    },
                    new Models.Review
                    {
                        Author = "Соломія Іваненко",
                        CityAndDate = "Харків · Січень 2025",
                        Text = "Персонал дуже привітний, а сам готель — як щось з майбутнього.",
                        Rating = 5,
                        AvatarLetter = "С"
                    }
                };
            }

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
                        Price = PriceOf("single", 890),
                        ImageClass = "modal-img--standard",
                        // Amenities = new List<string>
                        // {
                        //     "🌡️ Клімат-контроль",
                        //     "💡 LED-освітлення",
                        //     "🔌 USB + розетка",
                        //     "📶 Wi-Fi 300 Мбіт/с"
                        // }
                    },
                    new Capsule
                    {
                        Id = "singlew",
                        Title = "Одномісна капсула",
                        Subtitle = "Жіночий блок",
                        Description = "Спеціально для жінок — капсула з підвищеним рівнем комфорту та безпеки.",
                        Price = PriceOf("singlew", 1390),
                        ImageClass = "modal-img--premium",
                        // Amenities = new List<string>
                        // {
                        //     "🌡️ Клімат-контроль",
                        //     "🌈 RGB-підсвітка",
                        //     "🔌 USB-C + QI зарядка",
                        //     "📺 Smart TV 24\""
                        // }
                    },
                    new Capsule
                    {
                        Id = "singlem",
                        Title = "Одномісна капсула",
                        Subtitle = "Чоловічий блок",
                        Description = "Для чоловіків, які цінують комфорт і стиль — капсула з сучасними зручностями та унікальним дизайном.",
                        Price = PriceOf("singlem", 2190),
                        ImageClass = "modal-img--lux",
                        // Amenities = new List<string>
                        // {
                        //     "🌡️ Multi-zone клімат",
                        //     "🌌 Зоряний купол",
                        //     "🔊 Аудіосистема Bose",
                        //     "🍳 Мінікухня"
                        // }
                    },
                    new Capsule
                    {
                        Id = "double",
                        Title = "Двомісна капсула",
                        Subtitle = "Загальний блок",
                        Description = "Простора капсула для двох.",
                        Price = PriceOf("double", 2190),
                        ImageClass = "modal-img--double",
                        // Amenities = new List<string>
                        // {
                        //     "🌡️ Клімат-контроль",
                        //     "💡 LED-освітлення",
                        //     "🔌 USB + розетка",
                        //     "📶 Wi-Fi 300 Мбіт/с"
                        // }
                    }
                },
                Reviews = viewReviews
            };
        }
    }
}
