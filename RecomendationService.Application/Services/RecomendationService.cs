using Microsoft.Extensions.Logging;
using RecomendationService.Application.HttpClientContracts;
using RecomendationService.Application.IServiceContracts;
using RecomendationService.Application.RepositoryContracts;
using RecomendationService.Domain;

namespace RecomendationService.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ILogger<RecommendationService> _logger;
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly IProfileInfoProvider _profileInfoProvider;
        public RecommendationService(ILogger<RecommendationService> logger, IUserActivityRepository userActivityRepository, IProfileInfoProvider profileInfoProvider)
        {
            _logger = logger;
            _userActivityRepository = userActivityRepository;
            _profileInfoProvider = profileInfoProvider;
        }
        public async Task<Result<IEnumerable<Profile>>> GetRecomendationsAsync(Guid userID, int countOfUsers = 30)
        {
            if(userID == Guid.Empty)
            {
                _logger.LogError("GetRecomendationsAsync: userID is empty");
                return Result<IEnumerable<Profile>>.Failure(400, "userID is empty");
            }

            var result =  await _profileInfoProvider.GetProfileByIDAsync(userID);

            if(result.StatusCode == 404)
            {
                _logger.LogError("GetRecomendationsAsync: User with {userID} is not found", userID);
                return Result<IEnumerable<Profile>>.Failure(404, $"User with {userID} is not found");
            }

            if(!result.IsSuccess)
            {
                _logger.LogError("GetRecomendationsAsync: Error while getting profile for user with {userID}", userID);
                return Result<IEnumerable<Profile>>.Failure(500, "Error while getting profile");
            }

            Profile currentProfile = result.Value;

            var userActivitiesLikesResult = await _userActivityRepository.GetLikedUsersAsync(userID);

            var userActivitiesDislikesResult = await _userActivityRepository.GetDislikedUsersAsync(userID);

            var userActivitiesLikedUsersResult = await _userActivityRepository.GetLikesFromUsersAsync(userID);

            if (userActivitiesDislikesResult.StatusCode == 404 
                || userActivitiesDislikesResult.StatusCode == 404 
                || userActivitiesLikedUsersResult.StatusCode == 404)
            {
                _logger.LogError("GetRecomendationsAsync: User activity with {userID} is not found", userID);
                return Result<IEnumerable<Profile>>.Failure(404, $"Activity with {userID} is not found");
            }

            if (!userActivitiesLikesResult.IsSuccess 
                || !userActivitiesDislikesResult.IsSuccess 
                || !userActivitiesLikedUsersResult.IsSuccess)
            {
                _logger.LogError("GetRecomendationsAsync: Error while getting user activities for user with {userID}", userID);
                return Result<IEnumerable<Profile>>.Failure(500, "Error while getting user activities");
            }

            List<Guid> exludedGuids = userActivitiesLikesResult.Value
                .Concat(userActivitiesDislikesResult.Value)
                .Concat(userActivitiesLikedUsersResult.Value)
                .Append(userID)
                .ToList();

            var profilesResult = await _profileInfoProvider.GetProfilesByFilterAsync(exludedGuids, currentProfile.Gender);

            if(!profilesResult.IsSuccess)
            {
                _logger.LogError("GetRecomendationsAsync: Error while getting profiles for user with {userID}", userID);
                return Result<IEnumerable<Profile>>.Failure(profilesResult.StatusCode, profilesResult.ErrorMessage);
            }

            var profilesMatch = CountMatchedInterests(currentProfile, profilesResult.Value);

            var sortedProfiles = profilesMatch
                .OrderByDescending(x => x.Item2)
                .Select(x => x.Item1)
                .Take(countOfUsers);
            
            _logger.LogInformation("GetRecomendationsAsync: Successfully retrieved {Count} recommendations for userID {UserID}", sortedProfiles.Count(), userID);
            return Result<IEnumerable<Profile>>.Success(sortedProfiles);
        }

        private List<(Profile, int)> CountMatchedInterests(Profile currentProfile, IEnumerable<Profile> profiles)
        {
            List<(Profile, int)> profilesMatch = profiles.Select(x => (x, 0)).ToList();

            var interests = typeof(Profile).GetProperties().Where(x => x.Name.EndsWith("Interest"));

            for(int i = 0; i < profilesMatch.Count; i++)
            {
                int matchCount = 0;

                foreach (var interest in interests)
                {
                    var currentProfileInterest = interest.GetValue(currentProfile);
                    var profileInterest = interest.GetValue(profilesMatch[i].Item1);

                    if (currentProfileInterest != null && profileInterest != null && currentProfileInterest.Equals(profileInterest))
                    {
                        matchCount++;
                    }
                }

                profilesMatch[i] = (profilesMatch[i].Item1, matchCount);
            }

            return profilesMatch;
        }
    }
}
