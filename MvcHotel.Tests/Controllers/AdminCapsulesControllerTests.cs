using Microsoft.AspNetCore.Mvc;
using MvcHotel.Areas.Admin.Controllers;
using MvcHotel.Data.Entities;
using MvcHotel.Tests.Helpers;

namespace MvcHotel.Tests.Controllers
{
    public class AdminCapsulesControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithCapsulePrices()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var price = new CapsulePrice { Slug = "single", Price = 890 };
            context.CapsulePrices.Add(price);
            await context.SaveChangesAsync();

            var controller = new CapsulesController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CapsulePrice>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(890, model.First().Price);
        }

        [Fact]
        public async Task Index_ReturnsCapsuleNames_InViewBag()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new CapsulesController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(controller.ViewBag.CapsuleNames);
            var names = (Dictionary<string, string>)controller.ViewBag.CapsuleNames;
            Assert.Contains("single", names.Keys);
        }

        [Fact]
        public async Task Edit_GetWithValidId_ReturnsCapsuleData()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var price = new CapsulePrice { Id = 1, Slug = "double", Price = 1200 };
            context.CapsulePrices.Add(price);
            await context.SaveChangesAsync();

            var controller = new CapsulesController(context);

            // Act
            var result = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CapsulePrice>(viewResult.Model);
            Assert.Equal(1200, model.Price);
            Assert.Equal("double", model.Slug);
        }

        [Fact]
        public async Task Edit_GetWithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new CapsulesController(context);

            // Act
            var result = await controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_PostWithValidData_UpdatesPrice()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var price = new CapsulePrice { Id = 1, Slug = "single", Price = 890 };
            context.CapsulePrices.Add(price);
            await context.SaveChangesAsync();

            var controller = new CapsulesController(context);
            var updatedCapsule = new CapsulePrice { Id = 1, Slug = "single", Price = 950 };

            // Act
            var result = await controller.Edit(1, updatedCapsule);

            // Assert - TempData requires HttpContext, so just verify DB was updated
            var updated = context.CapsulePrices.First(c => c.Id == 1);
            Assert.Equal(950, updated.Price);
        }

        [Fact]
        public async Task Edit_PostWithNegativePrice_ReturnsViewWithError()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var price = new CapsulePrice { Id = 1, Slug = "single", Price = 890 };
            context.CapsulePrices.Add(price);
            await context.SaveChangesAsync();

            var controller = new CapsulesController(context);
            var invalidCapsule = new CapsulePrice { Id = 1, Slug = "single", Price = -100 };

            // Act
            var result = await controller.Edit(1, invalidCapsule);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("Price"));
        }

        [Fact]
        public async Task Edit_PostWithZeroPrice_ReturnsViewWithError()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var price = new CapsulePrice { Id = 1, Slug = "single", Price = 890 };
            context.CapsulePrices.Add(price);
            await context.SaveChangesAsync();

            var controller = new CapsulesController(context);
            var invalidCapsule = new CapsulePrice { Id = 1, Slug = "single", Price = 0 };

            // Act
            var result = await controller.Edit(1, invalidCapsule);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Edit_PostWithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var controller = new CapsulesController(context);
            var capsule = new CapsulePrice { Id = 5, Slug = "single", Price = 890 };

            // Act
            var result = await controller.Edit(1, capsule);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
