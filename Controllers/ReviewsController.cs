using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            var newReview = new Review
            {
                AuthorName = review.AuthorName,
                Text = review.Text
            };
            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}