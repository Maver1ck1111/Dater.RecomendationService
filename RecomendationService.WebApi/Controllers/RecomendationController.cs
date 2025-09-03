using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomendationService.Application;
using RecomendationService.Application.IServiceContracts;
using RecomendationService.Domain;

namespace RecomendationService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecomendationController : ControllerBase
    {
        private readonly IRecommendationService _recomendationService;
        private readonly ILogger<RecomendationController> _logger;
        public RecomendationController(IRecommendationService recomendationService, ILogger<RecomendationController> logger)
        {
            _recomendationService = recomendationService;
            _logger = logger;
        }

        [HttpGet("recomendationByUser")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetRecomendationByUser([FromQuery] Guid userId, [FromQuery] int countOfUsers = 30)
        {
            if(userId == Guid.Empty)
            {
                _logger.LogError("GetRecomendationByUser: userId is empty");
                return BadRequest("UserID cannot be empty");
            }

            _logger.LogInformation("GetRecomendationByUser: Received request for userId {UserID} with countOfUsers {CountOfUsers}", userId, countOfUsers);

            Result<IEnumerable<Profile>> result = await _recomendationService.GetRecomendationsAsync(userId, countOfUsers);

            if(result.StatusCode == 404)
            {
                _logger.LogError($"GetRecomendationByUser: {result.ErrorMessage}");
                return NotFound(result.ErrorMessage);
            }

            if(!result.IsSuccess)
            {
                _logger.LogError($"GetRecomendationByUser: {result.ErrorMessage}");
                return Problem(detail: result.ErrorMessage, statusCode: result.StatusCode);
            }

            return Ok(result.Value);
        }
    }
}
