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
        public RecommendationService(ILogger<RecommendationService> logger, IUserActivityRepository userActivityRepository, IProfileInfoProvider)
        {
            _logger = logger;
            _userActivityRepository = userActivityRepository;
            _userActivityRepository = userActivityRepository;
        }
        public Task<Result<IEnumerable<Profile>>> GetRecomendationsAsync(Guid userID)
        {
            throw new NotImplementedException();
        }
    }
}
