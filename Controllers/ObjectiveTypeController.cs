using Microsoft.AspNetCore.Mvc;
using TravelsBE.Entity;
using TravelsBE.Services.Interfaces;

namespace TravelsBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectiveTypeController : ControllerBase
    {
        private readonly IObjectiveTypeService _objectiveTypeService;

        public ObjectiveTypeController(IObjectiveTypeService objectiveTypeService)
        {
            _objectiveTypeService = objectiveTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _objectiveTypeService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _objectiveTypeService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] ObjectiveType objectiveType)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _objectiveTypeService.CreateAsync(objectiveType);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ObjectiveType updatedObjectiveType)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _objectiveTypeService.UpdateAsync(id, updatedObjectiveType);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _objectiveTypeService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }

}
