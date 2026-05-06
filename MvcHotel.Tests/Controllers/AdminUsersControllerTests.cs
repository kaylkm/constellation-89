using Microsoft.AspNetCore.Mvc;
using MvcHotel.Areas.Admin.Controllers;
using MvcHotel.Data.Entities;
using MvcHotel.Tests.Helpers;
using MvcHotel.ViewModels.Admin;

namespace MvcHotel.Tests.Controllers
{
    public class AdminUsersControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAllUsers_OrderedByName()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user1 = new User { Name = "Zebra" };
            var user2 = new User { Name = "Alice" };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var users = Assert.IsAssignableFrom<List<User>>(viewResult.Model);
            Assert.Equal(2, users.Count);
            Assert.Equal("Alice", users.First().Name);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsUserWithReviews()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "John Doe" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var review = new Review
            {
                AuthorId = user.Id,
                Text = "Great place",
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            // Act
            var result = await controller.Details(user.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedUser = Assert.IsType<User>(viewResult.Model);
            Assert.Equal("John Doe", returnedUser.Name);
            Assert.Single(returnedUser.Reviews);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_GetReturnsView()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<UserEditViewModel>(viewResult.Model);
            Assert.Equal("", vm.Name);
        }

        [Fact]
        public async Task Create_PostWithValidName_CreatesUser()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);
            var vm = new UserEditViewModel { Name = "New User" };

            // Act
            var result = await controller.Create(vm);

            // Assert - TempData requires HttpContext
            var createdUser = context.Users.FirstOrDefault(u => u.Name == "New User");
            Assert.NotNull(createdUser);
        }

        [Fact]
        public async Task Create_PostWithEmptyName_ReturnsViewWithError()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);
            var vm = new UserEditViewModel { Name = "" };

            controller.ModelState.AddModelError("Name", "Ім'я обов'язкове");

            // Act
            var result = await controller.Create(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_PostWithNameExceedingMaxLength_ReturnsViewWithError()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);
            var longName = new string('a', 256);
            var vm = new UserEditViewModel { Name = longName };

            controller.ModelState.AddModelError("Name", "Ім'я занадто довге");

            // Act
            var result = await controller.Create(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Edit_GetWithValidId_ReturnsUserData()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Test User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            // Act
            var result = await controller.Edit(user.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<UserEditViewModel>(viewResult.Model);
            Assert.Equal("Test User", vm.Name);
            Assert.Equal(user.Id, vm.Id);
        }

        [Fact]
        public async Task Edit_GetWithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);

            // Act
            var result = await controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_PostWithValidData_UpdatesUser()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Old Name" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);
            var vm = new UserEditViewModel { Id = user.Id, Name = "New Name" };

            // Act
            var result = await controller.Edit(user.Id, vm);

            // Assert - TempData requires HttpContext
            var updated = context.Users.First(u => u.Id == user.Id);
            Assert.Equal("New Name", updated.Name);
        }

        [Fact]
        public async Task Edit_PostWithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);
            var vm = new UserEditViewModel { Id = 5, Name = "User" };

            // Act
            var result = await controller.Edit(1, vm);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Edit_PostWithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new UsersController(context);
            var vm = new UserEditViewModel { Id = 999, Name = "User" };

            // Act
            var result = await controller.Edit(999, vm);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_PostWithEmptyName_ReturnsViewWithError()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "Original" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);
            var vm = new UserEditViewModel { Id = user.Id, Name = "" };
            controller.ModelState.AddModelError("Name", "Ім'я обов'язкове");

            // Act
            var result = await controller.Edit(user.Id, vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Delete_GetWithValidId_ReturnsDeleteConfirmation()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User to Delete" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UsersController(context);

            // Act
            var result = await controller.Delete(user.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedUser = Assert.IsType<User>(viewResult.Model);
            Assert.Equal(user.Id, returnedUser.Id);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesUser()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var user = new User { Name = "User to Delete" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userId = user.Id;
            var controller = new UsersController(context);

            // Act
            var result = await controller.DeleteConfirmed(userId);

            // Assert - TempData requires HttpContext
            var deleted = context.Users.FirstOrDefault(u => u.Id == userId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteConfirmed_WithCascadeDelete_RemovesUserReviews()
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

            var userId = user.Id;
            var controller = new UsersController(context);

            // Act
            var result = await controller.DeleteConfirmed(userId);

            // Assert
            Assert.Null(context.Users.FirstOrDefault(u => u.Id == userId));
            Assert.Empty(context.Reviews.Where(r => r.AuthorId == userId).ToList());
        }
    }
}
