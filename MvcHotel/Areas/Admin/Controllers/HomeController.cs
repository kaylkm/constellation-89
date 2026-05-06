using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;

namespace MvcHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly HotelDbContext _context;

        public HomeController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalReviews = await _context.Reviews.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalReplies = await _context.AdminReplies.CountAsync();
            ViewBag.AvgRating = await _context.Ratings
                .Where(r => r.GeneralImpression != null)
                .AverageAsync(r => (double?)r.GeneralImpression) ?? 0;

            var recentReviews = await _context.Reviews
                .Include(r => r.Author)
                .Include(r => r.Rating)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(recentReviews);
        }
    }
}
