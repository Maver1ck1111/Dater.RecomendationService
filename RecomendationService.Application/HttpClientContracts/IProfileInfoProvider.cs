using RecomendationService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Application.HttpClientContracts
{
    public interface IProfileInfoProvider
    {
        Task<Result<IEnumerable<Profile>>> GetProfilesByFilterAsync(IEnumerable<Guid> filterGuids);
        Task<Result<Profile>> GetProfileByIDAsync(Guid userID);
    }
}
