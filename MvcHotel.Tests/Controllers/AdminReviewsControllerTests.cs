using Microsoft.AspNetCore.Mvc;
using MvcHotel.Areas.Admin.Controllers;
using MvcHotel.Data.Entities;
using MvcHotel.Tests.Helpers;
using MvcHotel.ViewModels.Admin;

namespace MvcHotel.Tests.Controllers
{
    public class AdminReviewsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAllReviews_OrderedByDateDescending()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Test User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review1 = new Review
            {
                AuthorId = user.Id,
                Text = "First review",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-2)
            };
            var review2 = new Review
            {
                AuthorId = user.Id,
                Text = "Second review",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.AddRange(review1, review2);
            await context.SaveChangesAsync();

            var controller = new ReviewsController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var reviews = Assert.IsAssignableFrom<List<Review>>(viewResult.Model);
            Assert.Equal(2, reviews.Count);
            Assert.Equal("Second review", reviews.First().Text);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsReviewWithRelations()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Test User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Great experience",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var controller = new ReviewsController(context);

            // Act
            var result = await controller.Details(review.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedReview = Assert.IsType<Review>(viewResult.Model);
            Assert.Equal("Great experience", returnedReview.Text);
            Assert.NotNull(returnedReview.Author);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new ReviewsController(context);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_GetWithValidId_ReturnsReviewEditViewModel()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "John Doe" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Good place",
                CreatedAt = DateTimeOffset.UtcNow
            };
            var rating = new Rating
            {
                ReviewId = review.Id,
                GeneralImpression = (short)5
            };
            context.Reviews.Add(review);
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();

            var controller = new ReviewsController(context);

            // Act
            var result = await controller.Edit(review.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<ReviewEditViewModel>(viewResult.Model);
            Assert.Equal("John Doe", vm.AuthorName);
            Assert.Equal("Good place", vm.ReviewText);
            Assert.Equal((short)5, vm.GeneralImpression);
        }

        [Fact]
        public async Task Edit_PostAddsNewAdminReply()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Review text",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var controller = new ReviewsController(context);
            var vm = new ReviewEditViewModel
            {
                Id = review.Id,
                AuthorName = "User",
                ReviewText = "Review text",
                CreatedAt = DateTimeOffset.UtcNow,
                AdminReplyText = "Thank you for feedback!"
            };

            // Act
            var result = await controller.Edit(review.Id, vm);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var updatedReview = context.Reviews
                .Where(r => r.Id == review.Id)
                .FirstOrDefault();
            Assert.NotNull(updatedReview?.AdminReply);
            Assert.Equal("Thank you for feedback!", updatedReview.AdminReply.Text);
        }

        [Fact]
        public async Task Delete_GetWithValidId_ReturnsDeleteConfirmation()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Review",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var controller = new ReviewsController(context);

            // Act
            var result = await controller.Delete(review.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedReview = Assert.IsType<Review>(viewResult.Model);
            Assert.Equal(review.Id, returnedReview.Id);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesReviewAndRelatedData()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Review",
                CreatedAt = DateTimeOffset.UtcNow
            };
            var rating = new Rating
            {
                ReviewId = review.Id,
                GeneralImpression = (short)5
            };
            context.Reviews.Add(review);
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();

            var reviewId = review.Id;
            var controller = new ReviewsController(context);

            // Act
            var result = await controller.DeleteConfirmed(reviewId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(context.Reviews.FirstOrDefault(r => r.Id == reviewId));
            Assert.Null(context.Ratings.FirstOrDefault(r => r.ReviewId == reviewId));
        }

        [Fact]
        public async Task Edit_PostWithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new ReviewsController(context);
            var vm = new ReviewEditViewModel { Id = 5 };

            // Act
            var result = await controller.Edit(1, vm);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
