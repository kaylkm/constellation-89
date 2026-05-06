using Microsoft.AspNetCore.Mvc;
using MvcHotel.Areas.Admin.Controllers;
using MvcHotel.Data.Entities;
using MvcHotel.Tests.Helpers;

namespace MvcHotel.Tests.Controllers
{
    public class AdminHomeControllerTests
    {
        [Fact]
        public async Task Index_CalculatesTotalReviews()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review1 = new Review { AuthorId = user.Id, Text = "Review 1", CreatedAt = DateTimeOffset.UtcNow };
            var review2 = new Review { AuthorId = user.Id, Text = "Review 2", CreatedAt = DateTimeOffset.UtcNow };
            var review3 = new Review { AuthorId = user.Id, Text = "Review 3", CreatedAt = DateTimeOffset.UtcNow };
            context.Reviews.AddRange(review1, review2, review3);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(3, controller.ViewBag.TotalReviews);
        }

        [Fact]
        public async Task Index_CalculatesTotalUsers()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user1 = new User { Name = "User 1" };
            var user2 = new User { Name = "User 2" };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(2, controller.ViewBag.TotalUsers);
        }

        [Fact]
        public async Task Index_CalculatesTotalAdminReplies()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review1 = new Review { AuthorId = user.Id, Text = "Review 1", CreatedAt = DateTimeOffset.UtcNow };
            var review2 = new Review { AuthorId = user.Id, Text = "Review 2", CreatedAt = DateTimeOffset.UtcNow };
            context.Reviews.AddRange(review1, review2);
            await context.SaveChangesAsync();

            var reply1 = new AdminReply { ReviewId = review1.Id, Text = "Reply 1", CreatedAt = DateTimeOffset.UtcNow };
            var reply2 = new AdminReply { ReviewId = review2.Id, Text = "Reply 2", CreatedAt = DateTimeOffset.UtcNow };
            context.AdminReplies.AddRange(reply1, reply2);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(2, controller.ViewBag.TotalReplies);
        }

        [Fact]
        public async Task Index_CalculatesAverageRating()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review1 = new Review { AuthorId = user.Id, Text = "Review 1", CreatedAt = DateTimeOffset.UtcNow };
            var review2 = new Review { AuthorId = user.Id, Text = "Review 2", CreatedAt = DateTimeOffset.UtcNow };
            var review3 = new Review { AuthorId = user.Id, Text = "Review 3", CreatedAt = DateTimeOffset.UtcNow };
            context.Reviews.AddRange(review1, review2, review3);
            await context.SaveChangesAsync();

            var rating1 = new Rating { ReviewId = review1.Id, GeneralImpression = 5 };
            var rating2 = new Rating { ReviewId = review2.Id, GeneralImpression = 4 };
            var rating3 = new Rating { ReviewId = review3.Id, GeneralImpression = 3 };
            context.Ratings.AddRange(rating1, rating2, rating3);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(4.0, controller.ViewBag.AvgRating); // (5+4+3)/3 = 4
        }

        [Fact]
        public async Task Index_CalculatesAverageRating_IgnoresNullValues()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review1 = new Review { AuthorId = user.Id, Text = "Review 1", CreatedAt = DateTimeOffset.UtcNow };
            var review2 = new Review { AuthorId = user.Id, Text = "Review 2", CreatedAt = DateTimeOffset.UtcNow };
            var review3 = new Review { AuthorId = user.Id, Text = "Review 3", CreatedAt = DateTimeOffset.UtcNow };
            context.Reviews.AddRange(review1, review2, review3);
            await context.SaveChangesAsync();

            var rating1 = new Rating { ReviewId = review1.Id, GeneralImpression = 5 };
            var rating2 = new Rating { ReviewId = review2.Id, GeneralImpression = null };
            var rating3 = new Rating { ReviewId = review3.Id, GeneralImpression = 3 };
            context.Ratings.AddRange(rating1, rating2, rating3);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(4.0, controller.ViewBag.AvgRating); // (5+3)/2 = 4, null is ignored
        }

        [Fact]
        public async Task Index_ReturnsZeroAverage_WhenNoRatings()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(0, controller.ViewBag.AvgRating);
        }

        [Fact]
        public async Task Index_ReturnsRecentReviews_LimitedToFive()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Add 7 reviews
            for (int i = 0; i < 7; i++)
            {
                var review = new Review
                {
                    AuthorId = user.Id,
                    Text = $"Review {i}",
                    CreatedAt = DateTimeOffset.UtcNow.AddHours(-i)
                };
                context.Reviews.Add(review);
            }
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var recentReviews = Assert.IsAssignableFrom<List<Review>>(viewResult.Model);
            Assert.Equal(5, recentReviews.Count);
        }

        [Fact]
        public async Task Index_ReturnsRecentReviewsInDescendingOrder()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var now = DateTimeOffset.UtcNow;
            var review1 = new Review { AuthorId = user.Id, Text = "Old Review", CreatedAt = now.AddDays(-2) };
            var review2 = new Review { AuthorId = user.Id, Text = "Recent Review", CreatedAt = now };
            context.Reviews.AddRange(review1, review2);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var recentReviews = Assert.IsAssignableFrom<List<Review>>(viewResult.Model);
            Assert.Equal("Recent Review", recentReviews.First().Text);
        }

        [Fact]
        public async Task Index_WithAllStatistics()
        {
            // Arrange - Create a complete scenario
            var context = DbContextHelper.CreateInMemoryContext();
            var user1 = new User { Name = "User 1" };
            var user2 = new User { Name = "User 2" };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            // Create reviews with ratings
            var review1 = new Review { AuthorId = user1.Id, Text = "Great", CreatedAt = DateTimeOffset.UtcNow.AddHours(-1) };
            var review2 = new Review { AuthorId = user2.Id, Text = "Good", CreatedAt = DateTimeOffset.UtcNow };
            context.Reviews.AddRange(review1, review2);
            await context.SaveChangesAsync();

            var rating1 = new Rating { ReviewId = review1.Id, GeneralImpression = 5 };
            var rating2 = new Rating { ReviewId = review2.Id, GeneralImpression = 5 };
            context.Ratings.AddRange(rating1, rating2);

            var reply = new AdminReply { ReviewId = review1.Id, Text = "Thank you", CreatedAt = DateTimeOffset.UtcNow };
            context.AdminReplies.Add(reply);
            await context.SaveChangesAsync();

            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(2, controller.ViewBag.TotalReviews);
            Assert.Equal(2, controller.ViewBag.TotalUsers);
            Assert.Equal(1, controller.ViewBag.TotalReplies);
            Assert.Equal(5.0, controller.ViewBag.AvgRating);

            var recentReviews = Assert.IsAssignableFrom<List<Review>>(((ViewResult)result).Model);
            Assert.Equal(2, recentReviews.Count);
        }
    }
}
