using Microsoft.EntityFrameworkCore;
using HotelBooking.Models;

namespace HotelBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<AdminReply> AdminReplies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Rating)
                .WithOne(rt => rt.Review)
                .HasForeignKey<Rating>(rt => rt.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.AdminReply)
                .WithOne(ar => ar.Review)
                .HasForeignKey<AdminReply>(ar => ar.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<AdminReply>()
                .Property(ar => ar.CreatedAt)
                .HasDefaultValueSql("now()");
        }
    }
}