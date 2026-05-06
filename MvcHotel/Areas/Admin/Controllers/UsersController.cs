using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;
using MvcHotel.ViewModels.Admin;

namespace MvcHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly HotelDbContext _context;

        public UsersController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Reviews)
                .OrderBy(u => u.Name)
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.Rating)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View(new UserEditViewModel());
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new User { Name = vm.Name };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Користувача створено.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(new UserEditViewModel { Id = user.Id, Name = user.Name });
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = vm.Name;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Користувача оновлено.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Користувача видалено.";
            return RedirectToAction(nameof(Index));
        }
    }
}
