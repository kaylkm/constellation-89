using Microsoft.AspNetCore.Mvc;
using MvcHotel.Controllers;
using MvcHotel.Data.Entities;
using MvcHotel.Tests.Helpers;
using MvcHotel.ViewModels;

namespace MvcHotel.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Index_ReturnsHomeIndexViewModelWithCapsules()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeIndexViewModel>(viewResult.Model);
            Assert.NotNull(model.Capsules);
            Assert.NotEmpty(model.Capsules);
        }

        [Fact]
        public async Task Index_ReturnsDefaultReviewsWhenNoneExist()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeIndexViewModel>(viewResult.Model);
            Assert.NotNull(model.Reviews);
            Assert.NotEmpty(model.Reviews);
            Assert.Equal(3, model.Reviews.Count);
        }

        [Fact]
        public async Task SubmitReview_WithValidData_SavesDataToDatabase()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);
            var model = new HomeIndexViewModel
            {
                ReviewForm = new ReviewFormViewModel
                {
                    Name = "Jane Doe",
                    Text = "Excellent experience!",
                    Rating = 5,
                    RatingClean = 5,
                    RatingStaff = 4,
                    RatingValue = 4
                }
            };

            // Act
            try
            {
                await controller.SubmitReview(model);
            }
            catch (NullReferenceException)
            {
                // TempData requires controller context, but we can verify the data was saved
            }

            // Assert - verify data was persisted even if TempData failed
            Assert.Single(context.Reviews);
            Assert.Single(context.Ratings);
            var user = context.Users.FirstOrDefault(u => u.Name == "Jane Doe");
            Assert.NotNull(user);
        }

        [Fact]
        public async Task SubmitReview_WithInvalidData_ReturnsView()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);
            var model = new HomeIndexViewModel { ReviewForm = new ReviewFormViewModel() };

            controller.ModelState.AddModelError("ReviewForm.Name", "Name is required");

            // Act
            var result = await controller.SubmitReview(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task SubmitReview_CreatesUserIfNotExists()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);
            var model = new HomeIndexViewModel
            {
                ReviewForm = new ReviewFormViewModel
                {
                    Name = "New User",
                    Text = "Awesome place!",
                    Rating = 5,
                    RatingClean = 4,
                    RatingStaff = 5,
                    RatingValue = 4
                }
            };

            // Act
            try
            {
                await controller.SubmitReview(model);
            }
            catch (NullReferenceException)
            {
                // TempData requires controller context - skip that part
            }

            // Assert
            var user = context.Users.FirstOrDefault(u => u.Name == "New User");
            Assert.NotNull(user);
            Assert.Single(context.Users);
        }

        [Fact]
        public async Task SubmitReview_CreatesReviewAndRating()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new HomeController(context);
            var model = new HomeIndexViewModel
            {
                ReviewForm = new ReviewFormViewModel
                {
                    Name = "Test User",
                    Text = "Test review text",
                    Rating = 5,
                    RatingClean = 4,
                    RatingStaff = 5,
                    RatingValue = 3
                }
            };

            // Act
            try
            {
                await controller.SubmitReview(model);
            }
            catch (NullReferenceException)
            {
                // TempData requires controller context - skip that part
            }

            // Assert
            Assert.Single(context.Reviews);
            Assert.Single(context.Ratings);

            var review = context.Reviews.First();
            var rating = context.Ratings.First();

            Assert.Equal("Test review text", review.Text);
            Assert.Equal((short)5, rating.GeneralImpression);
            Assert.Equal((short)4, rating.Cleanliness);
            Assert.Equal((short)5, rating.Staff);
            Assert.Equal((short)3, rating.PriceQuality);
        }
    }
}
