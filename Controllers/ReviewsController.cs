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

        // GET: /Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            return View(review);
        }

        // POST: /Reviews/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            var existing = await _context.Reviews.FindAsync(id);
            if (existing == null) return NotFound();
            existing.AuthorName = review.AuthorName;
            existing.Text = review.Text;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Reviews/Delete/5
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