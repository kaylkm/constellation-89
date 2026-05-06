using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;

namespace MvcHotel.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static HotelDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new HotelDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
