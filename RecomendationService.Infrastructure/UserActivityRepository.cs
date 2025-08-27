using Microsoft.Extensions.Logging;
using RecomendationService.Application.RepositoryContracts;
using MongoDB;
using MongoDB.Driver;
using RecomendationService.Domain;
using RecomendationService.Application;

namespace RecomendationService.Infrastructure
{
    public class UserActivityRepository : IUserActivityRepository
    {
        private readonly ILogger<UserActivityRepository> _logger;
        private readonly IMongoCollection<UserActivities> _collection;
        public UserActivityRepository(ILogger<UserActivityRepository> logger, IMongoCollection<UserActivities> collection) 
        {
            _logger = logger;
            _collection = collection;
        }

        public Task<Result> CreateUserActivityAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<Guid>>> GetDislikedUsersAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<Guid>>> GetLikedUsersAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RecordUserActivityAsync(Guid userId, string activityType)
        {
            throw new NotImplementedException();
        }
    }
}
