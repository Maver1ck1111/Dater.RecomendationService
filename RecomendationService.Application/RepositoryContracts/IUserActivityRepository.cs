using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Application.RepositoryContracts
{
    public interface IUserActivityRepository
    {
        Task RecordUserActivityAsync(Guid userId, string activityType);
    }
}
