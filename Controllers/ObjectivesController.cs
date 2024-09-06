using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [HttpGet]
        public async Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync()
        {
            var objectives = await _objectiveService.GetObjectivesAsync();
            return objectives;
        }

        [HttpGet("{id}")]
        public async Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id)

        {
            var objective = await _objectiveService.GetObjectiveByIdAsync(id);

            return objective;
        }

        [HttpPost]
        public async Task<ActionResult<ObjectiveModel>> PostObjective(ObjectiveModel objective)
        {
            var createdObjective = await _objectiveService.CreateObjectiveAsync(objective);

            return CreatedAtAction(nameof(GetObjectiveByIdAsync), new { id = createdObjective.Id }, createdObjective);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutObjective(int id, ObjectiveModel objective)
        {
            if (id != objective.Id)
            {
                return BadRequest();
            }

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
            var deleted = await _objectiveService.DeleteObjectiveAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
