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
            // Використовуємо .Include(r => r.User), бо властивість тепер називається так
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string authorName, string text)
        {
            // Логіка напарника: знайти юзера за іменем або створити нового
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == authorName);
            if (user == null)
            {
                user = new User { Name = authorName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var newReview = new Review
            {
                AuthorId = user.Id,
                Text = text,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Додаємо ваші методи Edit/Delete, адаптовані під нову модель
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var review = await _context.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string text)
        {
            var existing = await _context.Reviews.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Text = text;
            existing.EditedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}