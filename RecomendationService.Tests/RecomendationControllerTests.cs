using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RecomendationService.Application;
using RecomendationService.Application.IServiceContracts;
using RecomendationService.Domain;
using RecomendationService.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Tests
{
    public class RecomendationControllerTests
    {
        private readonly Mock<IRecommendationService> _recomendationServiceMock = new Mock<IRecommendationService>();
        private readonly Mock<ILogger<RecomendationController>> _loggerMock = new Mock<ILogger<RecomendationController>>();

        [Fact]
        public async Task GetRecomendations_ShouldReturnCorrectResponse()
        {
            List<Profile> response = new List<Profile>()
            {
                    new Profile()
                    {
                        AccountID = Guid.NewGuid(),
                    },
                    new Profile()
                    {
                        AccountID = Guid.NewGuid(),
                    }
            };

            _recomendationServiceMock.Setup(x => x.GetRecomendationsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(Result<IEnumerable<Profile>>.Success(response));

            var controller = new RecomendationController(_recomendationServiceMock.Object, _loggerMock.Object);

            var result = await controller.GetRecomendationByUser(Guid.NewGuid(), 30);

            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeAssignableTo<IEnumerable<Profile>>();
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetRecomendation_SholdReturn400BadRequest_EmptyUserID()
        {
            var controller = new RecomendationController(_recomendationServiceMock.Object, _loggerMock.Object);

            var result = await controller.GetRecomendationByUser(Guid.Empty, 30);

            result.Result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("UserID cannot be empty");
        }

        [Fact]
        public async Task GetRecomendation_ShouldReturn404NotFound_UserNotFound()
        {
            Guid userID = Guid.NewGuid();
            _recomendationServiceMock.Setup(x => x.GetRecomendationsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(Result<IEnumerable<Profile>>.Failure(404, $"User with {userID} is not found"));

            var controller = new RecomendationController(_recomendationServiceMock.Object, _loggerMock.Object);

            var result = await controller.GetRecomendationByUser(userID, 30);

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be($"User with {userID} is not found");
        }

        [Fact]
        public async Task GetRecomendation_ShouldReturn404NotFound_ActivityNotFound()
        {
            Guid userID = Guid.NewGuid();
            _recomendationServiceMock.Setup(x => x.GetRecomendationsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
               .ReturnsAsync(Result<IEnumerable<Profile>>.Failure(404, $"Activity with {userID} is not found"));

            var controller = new RecomendationController(_recomendationServiceMock.Object, _loggerMock.Object);

            var result = await controller.GetRecomendationByUser(userID, 30);

            result.Result.Should().BeOfType<NotFoundObjectResult>();

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be($"Activity with {userID} is not found");
        }

        [Fact]
        public async Task GetRecomendation_ShouldReturn500ServerError_ErrorWhileGettingProfile()
        {
            _recomendationServiceMock.Setup(x => x.GetRecomendationsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
              .ReturnsAsync(Result<IEnumerable<Profile>>.Failure(500, "Error while getting profile"));

            var controller = new RecomendationController(_recomendationServiceMock.Object, _loggerMock.Object);

            var result = await controller.GetRecomendationByUser(Guid.NewGuid(), 30);

            result.Result.Should().BeOfType<ObjectResult>();
            
            var problemDetails = result.Result as ObjectResult;
            problemDetails.StatusCode.Should().Be(500);

            problemDetails.Value.Should().BeOfType<ProblemDetails>();
            var pd = problemDetails.Value as ProblemDetails;
            pd.Detail.Should().Be("Error while getting profile");
        }

        [Fact]
        public async Task GetRecomendation_ShouldReturn500ServerError_ErrorWhileGettingActivities()
        {
            _recomendationServiceMock.Setup(x => x.GetRecomendationsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
              .ReturnsAsync(Result<IEnumerable<Profile>>.Failure(500, "Error while getting user activities"));

            var controller = new RecomendationController(_recomendationServiceMock.Object, _loggerMock.Object);

            var result = await controller.GetRecomendationByUser(Guid.NewGuid(), 30);

            result.Result.Should().BeOfType<ObjectResult>();

            var problemDetails = result.Result as ObjectResult;
            problemDetails.StatusCode.Should().Be(500);

            problemDetails.Value.Should().BeOfType<ProblemDetails>();
            var pd = problemDetails.Value as ProblemDetails;
            pd.Detail.Should().Be("Error while getting user activities");
        }
    }
}
