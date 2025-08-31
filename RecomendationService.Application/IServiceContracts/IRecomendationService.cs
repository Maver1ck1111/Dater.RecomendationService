using RecomendationService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Application.IServiceContracts
{
    public interface IRecommendationService
    {
        Task<Result<IEnumerable<Profile>>> GetRecomendationsAsync(Guid userID);
    }
}
