using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;
using MvcHotel.ViewModels.Admin;

namespace MvcHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly HotelDbContext _context;

        public ReviewsController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Author)
                .Include(r => r.Rating)
                .Include(r => r.AdminReply)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        // GET: Admin/Reviews/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Author)
                .Include(r => r.Rating)
                .Include(r => r.AdminReply)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            return View(review);
        }

        // GET: Admin/Reviews/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Author)
                .Include(r => r.Rating)
                .Include(r => r.AdminReply)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            var vm = new ReviewEditViewModel
            {
                Id = review.Id,
                AuthorName = review.Author.Name,
                ReviewText = review.Text,
                GeneralImpression = review.Rating?.GeneralImpression,
                CreatedAt = review.CreatedAt,
                AdminReplyText = review.AdminReply?.Text
            };

            return View(vm);
        }

        // POST: Admin/Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewEditViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var review = await _context.Reviews
                .Include(r => r.AdminReply)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(vm.AdminReplyText))
            {
                if (review.AdminReply == null)
                {
                    // Adding a NEW reply
                    var newReply = new AdminReply
                    {
                        ReviewId = review.Id,
                        Text = vm.AdminReplyText.Trim(),
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    _context.AdminReplies.Add(newReply);
                }
                else
                {
                    // UPDATING an existing reply
                    review.AdminReply.Text = vm.AdminReplyText.Trim();
                    review.AdminReply.CreatedAt = DateTimeOffset.UtcNow; // Optional: update the timestamp or keep original
                    _context.Update(review.AdminReply);
                }
                TempData["Success"] = "Відповідь успішно збережено.";
            }
            else if (review.AdminReply != null)
            {
                // REMOVING a reply if text is cleared
                _context.AdminReplies.Remove(review.AdminReply);
                TempData["Success"] = "Відповідь видалено.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Reviews/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Author)
                .Include(r => r.Rating)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            return View(review);
        }

        // POST: Admin/Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Explicitly find the review with its related rating and reply
            var review = await _context.Reviews
                .Include(r => r.Rating)
                .Include(r => r.AdminReply)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review != null)
            {
                // Remove the review (Rating and AdminReply will be removed automatically 
                // due to the One-to-One Cascade defined in HotelDbContext)
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Відгук #{id} успішно видалено.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
