using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: /Rooms

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        // GET: /Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);

            if (room == null)
                return NotFound();

            return View(room);
        }
		// GET: /Rooms/Book/5
		public async Task<IActionResult> Book(int? id)
		{
			if (id == null) return NotFound();
			var room = await _context.Rooms.FindAsync(id);
			if (room == null) return NotFound();
			ViewBag.Room = room;
            return View(new Booking { RoomId = id.Value });
        }

        // POST: /Rooms/Book
        [HttpPost]
        public async Task<IActionResult> Book(Booking booking)
        {
            var newBooking = new Booking
            {
                CustomerName = booking.CustomerName,
                Email = booking.Email,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                RoomId = booking.RoomId
            };
            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}