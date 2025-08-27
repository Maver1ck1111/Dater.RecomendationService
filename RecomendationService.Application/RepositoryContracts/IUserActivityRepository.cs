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
        Task<Result> RecordUserActivityAsync(Guid userId, Guid targetUserID, string activityType);
        Task<Result> CreateUserActivityAsync(Guid userId);
        Task<Result<IEnumerable<Guid>>> GetLikedUsersAsync(Guid userId);
        Task<Result<IEnumerable<Guid>>> GetDislikedUsersAsync(Guid userId);
    }
}
