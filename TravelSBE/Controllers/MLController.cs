using Microsoft.AspNetCore.Mvc;
using TravelSBE.Services;
using TravelSBE.Models.ML;

namespace TravelSBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MLController : ControllerBase
    {
        private readonly IMLService _mlService;

        public MLController(IMLService mlService)
        {
            _mlService = mlService;
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainModel()
        {
            try
            {
                await _mlService.TrainModelAsync();
                return Ok("Model antrenat cu succes");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Eroare la antrenarea modelului: {ex.Message}");
            }
        }

        [HttpGet("recommendations/{objectiveId}")]
        public async Task<IActionResult> GetRecommendations(int objectiveId, [FromQuery] int count = 5)
        {
            try
            {
                var recommendedObjectives = await _mlService.GetRecommendedObjectivesAsync(objectiveId, count);
                return Ok(recommendedObjectives);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Eroare la obținerea recomandărilor: {ex.Message}");
            }
        }
    }
} 