using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;

namespace MvcHotel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CapsulesController : Controller
    {
        private readonly HotelDbContext _context;

        private static readonly Dictionary<string, string> CapsuleNames = new()
        {
            ["single"]  = "Одномісна · Загальний блок",
            ["singlew"] = "Одномісна · Жіночий блок",
            ["singlem"] = "Одномісна · Чоловічий блок",
            ["double"]  = "Двомісна · Загальний блок"
        };

        public CapsulesController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Capsules
        public async Task<IActionResult> Index()
        {
            var prices = await _context.CapsulePrices
                .OrderBy(c => c.Id)
                .ToListAsync();

            ViewBag.CapsuleNames = CapsuleNames;
            return View(prices);
        }

        // GET: Admin/Capsules/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var capsule = await _context.CapsulePrices.FindAsync(id);
            if (capsule == null) return NotFound();

            ViewBag.CapsuleName = CapsuleNames.TryGetValue(capsule.Slug, out var name) ? name : capsule.Slug;
            return View(capsule);
        }

        // POST: Admin/Capsules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Slug,Price")] CapsulePrice capsule)
        {
            if (id != capsule.Id) return BadRequest();

            if (capsule.Price <= 0)
            {
                ModelState.AddModelError("Price", "Ціна повинна бути більше нуля.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CapsuleName = CapsuleNames.TryGetValue(capsule.Slug, out var n) ? n : capsule.Slug;
                return View(capsule);
            }

            var existing = await _context.CapsulePrices.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Price = capsule.Price;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Ціну капсули «{(CapsuleNames.TryGetValue(existing.Slug, out var nm) ? nm : existing.Slug)}» оновлено до {existing.Price} грн.";
            return RedirectToAction(nameof(Index));
        }
    }
}
