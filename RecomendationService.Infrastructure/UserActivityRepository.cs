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

        public async Task<Result> AddLikeFromUserAsync(Guid userId, Guid likedByUserId)
        {
            if(userId == Guid.Empty || likedByUserId == Guid.Empty)
            {
                _logger.LogError("AddLikeFromUserAsync: Ids can not be empty");
                return Result.Failure(400, "Ids can not be empty");
            }

            var filter = Builders<UserActivities>.Filter.Eq(ua => ua.UserActivitiesId, userId);

            var update = Builders<UserActivities>.Update.Push(ua => ua.LikesFromUsers, likedByUserId);

            var result = await _collection.UpdateOneAsync(filter, update);

            if(result.MatchedCount == 0)
            {
                _logger.LogError("AddLikeFromUserAsync: No activity found for UserId {UserId}", userId);
                return Result.Failure(404, "No activity found for this UserId");
            }

            if (result.ModifiedCount == 0)
            {
                _logger.LogWarning("AddLikeFromUserAsync: Like from UserId {LikedByUserId} already exists for UserId {UserId}", likedByUserId, userId);
                return Result.Failure(409, "Like from this UserId already exists");
            }

            _logger.LogInformation("AddLikeFromUserAsync: Successfully added like from UserId {LikedByUserId} to UserId {UserId}", likedByUserId, userId);
            return Result.Success();
        }

        public async Task<Result> CreateUserActivityAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                _logger.LogWarning("CreateUserActivityAsync: Id can not be empty");
                return Result.Failure(400, "Id can not be empty");
            }

            var existingActivity = await _collection.Find(ua => ua.UserActivitiesId == userId).FirstOrDefaultAsync();

            if(existingActivity != null)
            {
                _logger.LogError("CreateUserActivityAsync: Activity for UserId {UserId} already exists.", userId);
                return Result.Failure(409, "Activity already exists for this UserId.");
            }

            var newActivity = new UserActivities
            {
                UserActivitiesId = userId,
                LikedUsers = new List<Guid>(),
                DislikedUsers = new List<Guid>()
            };

            await _collection.InsertOneAsync(newActivity);

            _logger.LogInformation("CreateUserActivityAsync: Created new activity for UserId {UserId}.", userId);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<Guid>>> GetDislikedUsersAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                _logger.LogError("CreateUserActivityAsync: Id can not be empty");
                return Result<IEnumerable<Guid>>.Failure(400, "Id can not be null");
            }

            var activity = await _collection.Find(ua => ua.UserActivitiesId == userId).FirstOrDefaultAsync();

            if(activity == null)
            {
                _logger.LogError("GetDislikedUsersAsync: No activity found for UserId {UserId}.", userId);
                return Result<IEnumerable<Guid>>.Failure(404, "No activity found for this UserId.");
            }

            _logger.LogInformation("GetDislikedUsersAsync: Retrieved disliked users for UserId {UserId}", userId);
            return Result<IEnumerable<Guid>>.Success(activity.DislikedUsers);
        }

        public async Task<Result<IEnumerable<Guid>>> GetLikedUsersAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogError("CreateUserActivityAsync: Id can not be empty");
                return Result<IEnumerable<Guid>>.Failure(400, "Id can not be null");
            }

            var activity = await _collection.Find(ua => ua.UserActivitiesId == userId).FirstOrDefaultAsync();

            if (activity == null)
            {
                _logger.LogError("GetDislikedUsersAsync: No activity found for UserId {UserId}.", userId);
                return Result<IEnumerable<Guid>>.Failure(404, "No activity found for this UserId.");
            }

            _logger.LogInformation("GetDislikedUsersAsync: Retrieved liked users for UserId {UserId}.", userId);
            return Result<IEnumerable<Guid>>.Success(activity.LikedUsers);
        }

        public async Task<Result<IEnumerable<Guid>>> GetLikesFromUsersAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                _logger.LogError("GetLikesFromUsersAsync: Id can not be empty");
                return Result<IEnumerable<Guid>>.Failure(400, "Id can not be empty");
            }

            var filter = Builders<UserActivities>.Filter.Eq(ua => ua.UserActivitiesId, userId);

            var activity = await (await _collection.FindAsync(filter)).FirstOrDefaultAsync();

            if(activity == null)
            {
                _logger.LogError("GetLikesFromUsersAsync: No activity found for UserId {UserId}", userId);
                return Result<IEnumerable<Guid>>.Failure(404, "No activity found for this UserId");
            }

            _logger.LogInformation("GetLikesFromUsersAsync: Retrieved likes from users for UserId {UserId}", userId);
            return Result<IEnumerable<Guid>>.Success(activity.LikesFromUsers);
        }

        public async Task<Result> RecordUserActivityAsync(Guid userId, Guid targetUserID, string activityType)
        {
            if(userId == Guid.Empty)
            {
                _logger.LogError("RecordUserActivityAsync: Id can not be empty");
                return Result.Failure(400, "Id can not be empty");
            }

            if(activityType != "like" && activityType != "dislike")
            {
                _logger.LogError("RecordUserActivityAsync: Invalid activity type {ActivityType}", activityType);
                return Result.Failure(400, "Invalid activity type. Must be 'like' or 'dislike'");
            }

            var findFilter = Builders<UserActivities>.Filter.Eq(x => x.UserActivitiesId, userId);
            var activity = await _collection.Find(ua => ua.UserActivitiesId == userId).FirstOrDefaultAsync();

            if(activity == null)
            {
                _logger.LogError("RecordUserActivityAsync: No activity found for this {UserId}", userId);
                return Result.Failure(404, "No activity found for this UserId");
            }

            var updateLikedUsersFilter = Builders<UserActivities>.Update.Push(x => x.LikedUsers, targetUserID);
            var updateDislikedUsersFilter = Builders<UserActivities>.Update.Push(x => x.DislikedUsers, targetUserID);

            if(activityType == "like")
            {
                await _collection.UpdateOneAsync(findFilter, updateLikedUsersFilter);
            }
            else
            {
               await _collection.UpdateOneAsync(findFilter, updateDislikedUsersFilter);
            }

            _logger.LogInformation("RecordUserActivityAsync: succesfully added activity for {UserID}", userId);
            return Result.Success();
        }

        public async Task<Result> RemoveLikeFromUserAsync(Guid userId, Guid likedByUserId)
        {
            if(userId == Guid.Empty || likedByUserId == Guid.Empty)
            {
                _logger.LogError("RemoveLikeFromUserAsync: Ids can not be empty");
                return Result.Failure(400, "Ids can not be empty");
            }

            var filter = Builders<UserActivities>.Filter.Eq(ua => ua.UserActivitiesId, userId);
            var update = Builders<UserActivities>.Update.Pull(ua => ua.LikesFromUsers, likedByUserId);

            var result = await _collection.UpdateOneAsync(filter, update);

            if(result.MatchedCount == 0)
            {
                _logger.LogError("RemoveLikeFromUserAsync: No activity found for UserId {UserId}", userId);
                return Result.Failure(404, "No activity found for this UserId");
            }

            if (result.ModifiedCount == 0)
            {
                _logger.LogWarning("RemoveLikeFromUserAsync: Like from UserId {LikedByUserId} does not exist for UserId {UserId}", likedByUserId, userId);
                return Result.Failure(409, "Like from this UserId does not exist");
            }

            _logger.LogInformation("RemoveLikeFromUserAsync: Successfully removed like from UserId {LikedByUserId} for UserId {UserId}", likedByUserId, userId);
            return Result.Success();
        }
    }
}
