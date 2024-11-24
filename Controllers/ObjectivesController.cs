using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelsBE.Dtos;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectivesController : ControllerBase
    {
        private readonly IObjectiveService _objectiveService;

        public ObjectivesController(IObjectiveService objectiveService)
        {
            _objectiveService = objectiveService;
        }

        [HttpGet("GetObjectivesAsync")]
        public async Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync(string? search)
        {
            var objectives = await _objectiveService.GetObjectivesAsync(search);
            return objectives;
        }
        [HttpGet("GetLocalObjectives")]
        public async Task<ServiceResult<List<ObjectiveModel>>> GetLocalObjectives([FromQuery] GetLocalObjectivesRequest paylaod)
        {
            return await _objectiveService.GetLocalObjectives(paylaod.Latitude, paylaod.Latitude);
        }

        [HttpGet("{id}")]
        public async Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id)

        {
            var objective = await _objectiveService.GetObjectiveByIdAsync(id);

            return objective;
        }

        [HttpPost("PostObjective")]
        public async Task<ActionResult<ObjectiveModel>> PostObjective(ObjectiveModel objective)
        {
            var createdObjective = await _objectiveService.CreateObjectiveAsync(objective);

            return CreatedAtAction(nameof(GetObjectiveByIdAsync), new { id = createdObjective.Result.Id }, createdObjective);
        }

        [HttpPut("UpdateObjective")]
        public async Task<IActionResult> PutObjective([FromQuery] Objective objective)
        {
            var updatedObjective = await _objectiveService.UpdateObjectiveAsync(objective);

            if (updatedObjective == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObjective(int id)
        {
            var result = await _objectiveService.DeleteObjectiveAsync(id);

            if (!result.Result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
