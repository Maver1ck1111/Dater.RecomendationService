using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using RecomendationService.Domain;
using RecomendationService.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Tests
{
    public class RepositoryTests
    {
        private readonly Mock<ILogger<UserActivityRepository>> _mockLogger = new Mock<ILogger<UserActivityRepository>>();
        private readonly IMongoCollection<UserActivities> _activities;
        private readonly UserActivityRepository _repository;
        public RepositoryTests()
        {
            _activities = DbContext.GetCollection();
            _repository = new UserActivityRepository(_mockLogger.Object, _activities);
        }

        [Fact]
        public async Task CreateUserActivity_ShouldReturnCorrectResponse()
        {
            Guid activityID = Guid.NewGuid();

            var result = await _repository.CreateUserActivityAsync(activityID);

            result.StatusCode.Should().Be(200);

            var filter = Builders<UserActivities>.Filter.Eq(ua => ua.UserActivitiesId, activityID);

            var createdActivity = await (await _activities.FindAsync(filter)).FirstOrDefaultAsync();

            createdActivity.Should().NotBeNull();
            createdActivity.UserActivitiesId.Should().Be(activityID);
        }

        [Fact]
        public async Task CreateUserActivity_ShouldReturn400BadRequest_EmptyUserID()
        {
            var result = await _repository.CreateUserActivityAsync(Guid.Empty);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task CreateUserActivity_ShouldReturn409Conflict_ActivityAlreadyExist()
        {
            Guid activityID = Guid.NewGuid();

            await _activities.InsertOneAsync(new UserActivities { UserActivitiesId = activityID });

            var result = await _repository.CreateUserActivityAsync(activityID);
            result.StatusCode.Should().Be(409);
        }

        [Fact]
        public async Task RecordUserActivity_ShouldReturnCorrectResponse()
        {
            Guid activityID = Guid.NewGuid();
            Guid targetUserID = Guid.NewGuid();

            await _activities.InsertOneAsync(new UserActivities { UserActivitiesId = activityID });

            var result = await _repository.RecordUserActivityAsync(activityID, targetUserID, "like");

            result.StatusCode.Should().Be(200);

            var filter = Builders<UserActivities>.Filter.Eq(ua => ua.UserActivitiesId, activityID);

            var recorderActivity = await (await _activities.FindAsync(filter)).FirstOrDefaultAsync();

            recorderActivity.Should().NotBeNull();
            recorderActivity.UserActivitiesId.Should().Be(activityID);
            recorderActivity.LikedUsers.Should().Contain(targetUserID);
        }

        [Fact]
        public async Task RecordUserActivity_ShouldReturn400BadRequest_WrongActivityType()
        {
            Guid activityID = Guid.NewGuid();

            await _activities.InsertOneAsync(new UserActivities { UserActivitiesId = activityID });

            var result = await _repository.RecordUserActivityAsync(activityID, Guid.NewGuid(), "wrongType");
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task RecordUserActivity_ShouldReturn404NotFound_ActivityNotExist()
        {
            var result = await _repository.RecordUserActivityAsync(Guid.NewGuid(), Guid.NewGuid(), "like");
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetLikedUsersAsync_ShouldReturnCorrectResponse()
        {
            Guid activityID = Guid.NewGuid();
            Guid likedPersonID = Guid.NewGuid();

            await _activities.InsertOneAsync(new UserActivities { UserActivitiesId = activityID, LikedUsers = new List<Guid>() { likedPersonID } });

            var result = await _repository.GetLikedUsersAsync(activityID);

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
            result.Value.Should().Contain(likedPersonID);
        }

        [Fact]
        public async Task GetLikedUsersAsync_ShouldReturn400BadRequest_EmptyId()
        {
            var result = await _repository.GetLikedUsersAsync(Guid.Empty);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetLikedUsersAsync_ShouldReturn404NotFound_ActivityNotExist()
        {
            var result = await _repository.GetLikedUsersAsync(Guid.NewGuid());
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetDislikedUsersAsync_ShouldReturnCorrectResponse()
        {
            Guid activityID = Guid.NewGuid();
            Guid disLikedPersonID = Guid.NewGuid();

            await _activities.InsertOneAsync(new UserActivities { UserActivitiesId = activityID, DislikedUsers = new List<Guid>() { disLikedPersonID } });

            var result = await _repository.GetDislikedUsersAsync(activityID);

            result.StatusCode.Should().Be(200);
            result.Value.Should().NotBeNull();
            result.Value.Should().Contain(disLikedPersonID);
        }

        [Fact]
        public async Task GetDislikedUsersAsync_ShouldReturn400BadRequest_EmptyId()
        {
            var result = await _repository.GetDislikedUsersAsync(Guid.Empty);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetDislikedUsersAsync_ShouldReturn404NotFound_ActivityNotExist()
        {
            var result = await _repository.GetDislikedUsersAsync(Guid.NewGuid());
            result.StatusCode.Should().Be(404);
        }


    }
}
