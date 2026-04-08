using Microsoft.EntityFrameworkCore;

namespace MvcHotel.Data.Entities
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<AdminReply> AdminReplies { get; set; } = null!;
        public DbSet<CapsulePrice> CapsulePrices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AuthorId).HasColumnName("author_id");
                entity.Property(e => e.Text).IsRequired().HasColumnName("text");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
                entity.Property(e => e.EditedAt).HasColumnName("edited_at");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("ratings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReviewId).HasColumnName("review_id");

                entity.Property(e => e.GeneralImpression).HasColumnName("general_impression");
                entity.Property(e => e.Cleanliness).HasColumnName("cleanliness");
                entity.Property(e => e.Staff).HasColumnName("staff");
                entity.Property(e => e.PriceQuality).HasColumnName("price_quality");

                entity.HasOne(d => d.Review)
                    .WithOne(p => p.Rating)
                    .HasForeignKey<Rating>(d => d.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AdminReply>(entity =>
            {
                entity.ToTable("admin_replies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReviewId).HasColumnName("review_id");
                entity.Property(e => e.Text).IsRequired().HasColumnName("text");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

                entity.HasOne(d => d.Review)
                    .WithOne(p => p.AdminReply)
                    .HasForeignKey<AdminReply>(d => d.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CapsulePrice>(entity =>
            {
                entity.ToTable("capsule_prices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(50).HasColumnName("slug");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.HasIndex(e => e.Slug).IsUnique();
            });
        }
    }
}
