using MvcHotel.Data.Entities;
using MvcHotel.Models;
using ReviewEntity = MvcHotel.Data.Entities.Review;
using ReviewModel = MvcHotel.Models.Review;

namespace MvcHotel.Tests.Models
{
    public class CapsuleTests
    {
        [Fact]
        public void Capsule_CanBeCreated_WithValidData()
        {
            // Arrange & Act
            var capsule = new Capsule
            {
                Id = "single",
                Title = "Single Capsule",
                Subtitle = "General Block",
                Description = "A comfortable single capsule",
                Price = 890,
                ImageClass = "modal-img--standard",
                Badge = "New",
                BadgeClass = "badge-primary"
            };

            // Assert
            Assert.Equal("single", capsule.Id);
            Assert.Equal("Single Capsule", capsule.Title);
            Assert.Equal(890, capsule.Price);
            Assert.Equal("modal-img--standard", capsule.ImageClass);
        }

        [Fact]
        public void Capsule_HasEmptyStringDefaults()
        {
            // Arrange & Act
            var capsule = new Capsule();

            // Assert
            Assert.Equal("", capsule.Id);
            Assert.Equal("", capsule.Title);
            Assert.Equal("", capsule.Subtitle);
            Assert.Equal("", capsule.Description);
            Assert.Equal("", capsule.ImageClass);
            Assert.Equal("", capsule.Badge);
            Assert.Equal("", capsule.BadgeClass);
        }

        [Fact]
        public void Capsule_HasEmptyAmenitiesList()
        {
            // Arrange & Act
            var capsule = new Capsule();

            // Assert
            Assert.NotNull(capsule.Amenities);
            Assert.Empty(capsule.Amenities);
        }

        [Fact]
        public void Capsule_CanAddAmenities()
        {
            // Arrange
            var capsule = new Capsule();
            var amenities = new List<string> { "WiFi", "Air Conditioning", "USB Charger" };

            // Act
            capsule.Amenities = amenities;

            // Assert
            Assert.Equal(3, capsule.Amenities.Count);
            Assert.Contains("WiFi", capsule.Amenities);
        }
    }

    public class UserTests
    {
        [Fact]
        public void User_CanBeCreated_WithValidData()
        {
            // Arrange & Act
            var user = new User
            {
                Id = 1,
                Name = "John Doe"
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("John Doe", user.Name);
        }

        [Fact]
        public void User_HasEmptyNameDefault()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal("", user.Name);
        }

        [Fact]
        public void User_HasEmptyReviewsCollection()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.NotNull(user.Reviews);
            Assert.Empty(user.Reviews);
        }

        [Fact]
        public void User_CanHaveReviews()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Jane Doe" };
            var review = new ReviewEntity
            {
                Id = 1,
                AuthorId = 1,
                Text = "Great place!",
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Act
            user.Reviews.Add(review);

            // Assert
            Assert.Single(user.Reviews);
            Assert.Contains(review, user.Reviews);
        }
    }

    public class ReviewEntityTests
    {
        [Fact]
        public void ReviewEntity_CanBeCreated_WithValidData()
        {
            // Arrange & Act
            var now = DateTimeOffset.UtcNow;
            var review = new ReviewEntity
            {
                Id = 1,
                AuthorId = 1,
                Text = "Excellent experience!",
                CreatedAt = now
            };

            // Assert
            Assert.Equal(1, review.Id);
            Assert.Equal(1, review.AuthorId);
            Assert.Equal("Excellent experience!", review.Text);
            Assert.Equal(now, review.CreatedAt);
        }

        [Fact]
        public void ReviewEntity_HasEmptyTextDefault()
        {
            // Arrange & Act
            var review = new ReviewEntity();

            // Assert
            Assert.Equal("", review.Text);
        }

        [Fact]
        public void ReviewEntity_CanHaveRating()
        {
            // Arrange
            var review = new ReviewEntity
            {
                Id = 1,
                AuthorId = 1,
                Text = "Nice place",
                CreatedAt = DateTimeOffset.UtcNow
            };
            var rating = new Rating
            {
                Id = 1,
                ReviewId = 1,
                GeneralImpression = (short)5,
                Cleanliness = (short)4,
                Staff = (short)5,
                PriceQuality = (short)4
            };

            // Act
            review.Rating = rating;

            // Assert
            Assert.NotNull(review.Rating);
            Assert.Equal((short)5, review.Rating.GeneralImpression);
        }
    }

    public class ReviewModelTests
    {
        [Fact]
        public void ReviewModel_CanBeCreated_WithValidData()
        {
            // Arrange & Act
            var review = new ReviewModel
            {
                Author = "John Doe",
                CityAndDate = "Kyiv · May 2026",
                Text = "Amazing experience!",
                Rating = 5,
                AvatarLetter = "J",
                IsHighlighted = true
            };

            // Assert
            Assert.Equal("John Doe", review.Author);
            Assert.Equal(5, review.Rating);
            Assert.True(review.IsHighlighted);
        }

        [Fact]
        public void ReviewModel_CanHaveDetailedRatings()
        {
            // Arrange & Act
            var review = new ReviewModel
            {
                Author = "Jane Smith",
                Text = "Great place",
                Rating = 5,
                Cleanliness = 4,
                Staff = 5,
                PriceQuality = 4
            };

            // Assert
            Assert.Equal(4, review.Cleanliness);
            Assert.Equal(5, review.Staff);
            Assert.Equal(4, review.PriceQuality);
        }
    }

    public class RatingTests
    {
        [Fact]
        public void Rating_CanBeCreated_WithValidData()
        {
            // Arrange & Act
            var rating = new Rating
            {
                Id = 1,
                ReviewId = 1,
                GeneralImpression = (short)5,
                Cleanliness = (short)4,
                Staff = (short)5,
                PriceQuality = (short)4
            };

            // Assert
            Assert.Equal(1, rating.Id);
            Assert.Equal(1, rating.ReviewId);
            Assert.Equal((short)5, rating.GeneralImpression);
            Assert.Equal((short)4, rating.Cleanliness);
            Assert.Equal((short)5, rating.Staff);
            Assert.Equal((short)4, rating.PriceQuality);
        }

        [Fact]
        public void Rating_CanHaveNullValues()
        {
            // Arrange & Act
            var rating = new Rating
            {
                Id = 1,
                ReviewId = 1,
                GeneralImpression = (short)5,
                Cleanliness = null,
                Staff = null,
                PriceQuality = null
            };

            // Assert
            Assert.Equal((short)5, rating.GeneralImpression);
            Assert.Null(rating.Cleanliness);
            Assert.Null(rating.Staff);
            Assert.Null(rating.PriceQuality);
        }
    }
}
