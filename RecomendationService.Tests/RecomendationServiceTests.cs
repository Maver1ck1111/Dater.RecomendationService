using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecomendationService.Application;
using RecomendationService.Application.HttpClientContracts;
using RecomendationService.Application.RepositoryContracts;
using RecomendationService.Application.Services;
using RecomendationService.Domain;
using RecomendationService.Domain.Enums;
using System;


namespace RecomendationService.Tests
{
    public class RecomendationServiceTests
    {
        private readonly Mock<IProfileInfoProvider> _profileInfoProviderMock = new Mock<IProfileInfoProvider>();
        private readonly Mock<IUserActivityRepository> _userActivityRepositoryMock = new Mock<IUserActivityRepository>();
        private readonly Mock<ILogger<RecommendationService>> _loggerMock = new Mock<ILogger<RecommendationService>>();

        [Fact]
        public async Task GetRecomendationsAsync_ShouldReturnCorrectResponse()
        {
            Profile currentProfile = new Profile()
            {
                AccountID = Guid.NewGuid(),
                Name = "Test",
                DateOfBirth = new DateTime(2000, 10, 10),
                Description = "test description",
                Gender = Gender.Female,
                FoodInterest = FoodInterest.Italian,
                BookInterest = BookInterest.Fantasy,
                SportInterest = SportInterest.Football,
                MovieInterest = MovieInterest.Action,
                MusicInterest = MusicInterest.Rock,
                LifestyleInterest = LifestyleInterest.Minimalist,
            };

            _profileInfoProviderMock.Setup(x => x.GetProfileByIDAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<Profile>.Success(currentProfile));


            List<Profile> profilesList = new List<Profile>()
            {
                new Profile()
                {
                    AccountID = Guid.NewGuid(),
                    DateOfBirth = currentProfile.DateOfBirth.Date.AddYears(-3),
                },
                new Profile()
                {
                    AccountID = Guid.NewGuid(),
                    DateOfBirth = currentProfile.DateOfBirth.Date.AddYears(3),
                },
                new Profile()
                {
                    AccountID = Guid.NewGuid(),
                    DateOfBirth = currentProfile.DateOfBirth.Date.AddYears(1),
                    MovieInterest = currentProfile.MovieInterest,
                },
                new Profile()
                {
                    AccountID = Guid.NewGuid(),
                    DateOfBirth = currentProfile.DateOfBirth.Date.AddYears(-1),
                    MusicInterest = currentProfile.MusicInterest,
                    FoodInterest = currentProfile.FoodInterest
                },
                new Profile()
                {
                    AccountID = Guid.NewGuid(),
                    DateOfBirth = currentProfile.DateOfBirth.Date.AddYears(-1),
                    MusicInterest = currentProfile.MusicInterest,
                    FoodInterest = currentProfile.FoodInterest,
                    BookInterest = currentProfile.BookInterest
                }
            };

            _profileInfoProviderMock.Setup(x => x.GetProfilesByFilterAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<Gender>()))
                .ReturnsAsync(Result<IEnumerable<Profile>>.Success(profilesList));

            _userActivityRepositoryMock.Setup(x => x.GetLikedUsersAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<IEnumerable<Guid>>.Success(new List<Guid>()));

            _userActivityRepositoryMock.Setup(x => x.GetDislikedUsersAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<IEnumerable<Guid>>.Success(new List<Guid>()));

            _userActivityRepositoryMock.Setup(x => x.GetLikesFromUsersAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<IEnumerable<Guid>>.Success(new List<Guid>()));

            var service = new RecommendationService(_loggerMock.Object, _userActivityRepositoryMock.Object, _profileInfoProviderMock.Object);

            var result = await service.GetRecomendationsAsync(currentProfile.AccountID, 2);

            result.StatusCode.Should().Be(200);

            result.Value.Should().NotBeNull();

            result.Value.Should().ContainInConsecutiveOrder(profilesList[4], profilesList[3], profilesList[2]);
        }

        [Fact]
        public async Task GetRecomendationsAsync_ShouldReturn400BadRequest_EmptyId()
        {
            var service = new RecommendationService(_loggerMock.Object, _userActivityRepositoryMock.Object, _profileInfoProviderMock.Object);

            var result = await service.GetRecomendationsAsync(Guid.Empty, 2);

            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetRecomendationsAsync_ShouldReturn404NotFound_ProfileNotExist()
        {
            _profileInfoProviderMock.Setup(x => x.GetProfileByIDAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<Profile>.Failure(404, "No profile found for this userID"));

            var service = new RecommendationService(_loggerMock.Object, _userActivityRepositoryMock.Object, _profileInfoProviderMock.Object);

            var result = await service.GetRecomendationsAsync(Guid.NewGuid(), 2);

            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetRecomendationsAsync_ShouldReturn500InternalServerError_ProfileServiceFailure()
        {
            _profileInfoProviderMock.Setup(x => x.GetProfileByIDAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result<Profile>.Failure(500, "Profile service is down"));

            var service = new RecommendationService(_loggerMock.Object, _userActivityRepositoryMock.Object, _profileInfoProviderMock.Object);

            var result = await service.GetRecomendationsAsync(Guid.NewGuid(), 2);

            result.StatusCode.Should().Be(500);
        }
    }
}
