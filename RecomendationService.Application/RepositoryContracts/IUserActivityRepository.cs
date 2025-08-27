using RecomendationService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Application.RepositoryContracts
{
    public interface IUserActivityRepository
    {
        Task<Result> RecordUserActivityAsync(Guid userId, string activityType);
        Task<Result> CreateUserActivityAsync(Guid userId);
        Task<Result<List<Guid>>> GetLikedUsersAsync(Guid userId);
        Task<Result<List<Guid>>> GetDislikedUsersAsync(Guid userId);
    }
}
