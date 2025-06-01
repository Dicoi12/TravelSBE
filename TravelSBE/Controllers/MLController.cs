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
                await _mlService.UpdateClusterNeighborsAsync();
                return Ok(new { message = "Model antrenat cu succes și vecini actualizați" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("recommendations/{objectiveId}")]
        public async Task<IActionResult> GetRecommendations(int objectiveId, [FromQuery] int count = 4)
        {
            var result = await _mlService.GetRecommendedObjectivesAsync(objectiveId, count);
            if (result.Result == null)
                return NotFound(new { error = "Nu s-au găsit recomandări" });
            return Ok(result);
        }

        [HttpGet("visualization")]
        public async Task<IActionResult> GetVisualizationData()
        {
            var result = await _mlService.GetObjectivesForVisualizationAsync();
            if (result.Result == null)
                return NotFound(new { error = "Nu s-au găsit date pentru vizualizare" });
            return Ok(result);
        }

        [HttpPost("neighbors/update")]
        public async Task<IActionResult> UpdateNeighbors()
        {
            try
            {
                await _mlService.UpdateClusterNeighborsAsync();
                return Ok(new { message = "Vecini actualizați cu succes" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("neighbors/{objectiveId}")]
        public async Task<IActionResult> GetNearestNeighbors(int objectiveId, [FromQuery] int count = 4)
        {
            var result = await _mlService.GetNearestNeighborsAsync(objectiveId, count);
            if (result.Result == null)
                return NotFound(new { error = "Nu s-au găsit vecini" });
            return Ok(result);
        }

        [HttpGet("cluster-analysis")]
        public async Task<IActionResult> GetClusterAnalysis()
        {
            var result = await _mlService.GetClusterAnalysisAsync();
            return Ok(result);
        }
    }
} 