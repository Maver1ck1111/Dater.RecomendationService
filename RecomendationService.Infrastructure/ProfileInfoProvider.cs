using Microsoft.Extensions.Logging;
using RecomendationService.Application;
using RecomendationService.Application.HttpClientContracts;
using RecomendationService.Domain;
using System.Net.Http.Json;

namespace RecomendationService.Infrastructure
{
    public class ProfileInfoProvider : IProfileInfoProvider
    {
        private readonly ILogger<ProfileInfoProvider> _logger;
        private readonly HttpClient _client;
        public ProfileInfoProvider(ILogger<ProfileInfoProvider> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<Result<Profile>> GetProfileByIDAsync(Guid userID)
        {
            if(userID == Guid.Empty)
            {
                _logger.LogError("GetProfileByIDAsync: userID can not be empty");
                return Result<Profile>.Failure(400, "userID can not be empty");
            }

            var result = await _client.GetAsync($"getProfileByID/{userID}");

            if(!result.IsSuccessStatusCode)
            {
                _logger.LogError("GetProfileByIDAsync: Failed to retrieve profile for userID {UserID}. Status Code: {StatusCode}", userID, result.StatusCode);
                return Result<Profile>.Failure((int)result.StatusCode, "Failed to retrieve profile");
            }

            var profile = await result.Content.ReadFromJsonAsync<Profile>();

            if(profile == null)
            {
                _logger.LogWarning("GetProfileByIDAsync: No profile found for userID {UserID}", userID);
                return Result<Profile>.Failure(404, "No profile found for this userID");
            }

            _logger.LogInformation("GetProfileByIDAsync: Successfully retrieved profile for userID {UserID}", userID);
            return Result<Profile>.Success(profile);
        }

        public async Task<Result<IEnumerable<Profile>>> GetProfilesByFilterAsync(IEnumerable<Guid> filterGuids)
        {
            if(filterGuids == null || !filterGuids.Any())
            {
                _logger.LogWarning("GetProfilesByFilterAsync: filterGuids is null or empty");
                return Result<IEnumerable<Profile>>.Failure(400, "filterGuids can not be null or empty");
            }

            var result = await _client.PostAsJsonAsync("getProfilesByFilter", filterGuids);

            if(!result.IsSuccessStatusCode)
            {
                _logger.LogError("GetProfiles: Failed to retrieve profiles. Status Code: {StatusCode}", result.StatusCode);
                return Result<IEnumerable<Profile>>.Failure((int)result.StatusCode, "Failed to retrieve profiles");
            }

            var profiles = await result.Content.ReadFromJsonAsync<IEnumerable<Profile>>();

            _logger.LogInformation("GetProfiles: Successfully retrieved {Count} profiles", profiles?.Count() ?? 0);
            return Result<IEnumerable<Profile>>.Success(profiles ?? Enumerable.Empty<Profile>());
        }
    }
}
