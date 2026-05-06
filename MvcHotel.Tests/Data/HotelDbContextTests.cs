using MvcHotel.Data.Entities;
using MvcHotel.Tests.Helpers;

namespace MvcHotel.Tests.Data
{
    public class HotelDbContextTests
    {
        [Fact]
        public async Task AddUser_PersistsToDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Alice" };

            // Act
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var savedUser = context.Users.FirstOrDefault(u => u.Name == "Alice");
            Assert.NotNull(savedUser);
            Assert.Equal("Alice", savedUser.Name);
        }

        [Fact]
        public async Task AddReview_WithAuthor_PersistsToDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Bob" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Amazing experience!",
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Act
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            // Assert
            var savedReview = context.Reviews.FirstOrDefault(r => r.Text == "Amazing experience!");
            Assert.NotNull(savedReview);
            Assert.Equal(user.Id, savedReview.AuthorId);
        }

        [Fact]
        public async Task AddRating_PersistsToDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Charlie" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Good place",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var rating = new Rating
            {
                ReviewId = review.Id,
                GeneralImpression = (short)5,
                Cleanliness = (short)4,
                Staff = (short)5,
                PriceQuality = (short)4
            };

            // Act
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();

            // Assert
            var savedRating = context.Ratings.FirstOrDefault(r => r.ReviewId == review.Id);
            Assert.NotNull(savedRating);
            Assert.Equal((short)5, savedRating.GeneralImpression);
        }

        [Fact]
        public async Task UpdateUser_UpdatesDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Diana" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            user.Name = "Diana Updated";
            context.Users.Update(user);
            await context.SaveChangesAsync();

            // Assert
            var updated = context.Users.FirstOrDefault(u => u.Id == user.Id);
            Assert.NotNull(updated);
            Assert.Equal("Diana Updated", updated.Name);
        }

        [Fact]
        public async Task DeleteUser_RemovesFromDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Eve" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var userId = user.Id;

            // Act
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            // Assert
            var deleted = context.Users.FirstOrDefault(u => u.Id == userId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task AddCapsulePrice_PersistsToDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var price = new CapsulePrice
            {
                Slug = "single",
                Price = 890
            };

            // Act
            context.CapsulePrices.Add(price);
            await context.SaveChangesAsync();

            // Assert
            var saved = context.CapsulePrices.FirstOrDefault(p => p.Slug == "single");
            Assert.NotNull(saved);
            Assert.Equal(890, saved.Price);
        }

        [Fact]
        public async Task AddAdminReply_PersistsToDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Frank" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Good place",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var reply = new AdminReply
            {
                ReviewId = review.Id,
                Text = "Thank you for your feedback!",
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Act
            context.AdminReplies.Add(reply);
            await context.SaveChangesAsync();

            // Assert
            var saved = context.AdminReplies.FirstOrDefault(r => r.ReviewId == review.Id);
            Assert.NotNull(saved);
            Assert.Equal("Thank you for your feedback!", saved.Text);
        }

        [Fact]
        public async Task CascadeDelete_RemovesRelatedReviews()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Grace" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Nice experience",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            // Act
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            // Assert
            var reviews = context.Reviews.Where(r => r.AuthorId == user.Id).ToList();
            Assert.Empty(reviews);
        }
    }
}
