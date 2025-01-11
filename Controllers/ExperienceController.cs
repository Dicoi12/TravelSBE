using Microsoft.AspNetCore.Mvc;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;

namespace TravelsBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExperiences()
        {
            var result = await _experienceService.GetAllExperiencesAsync();
            if (result.Result == null || !result.Result.Any())
            {
                return NotFound("Nu s-au găsit experiențe.");
            }

            return Ok(result.Result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExperienceById(int id)
        {
            var result = await _experienceService.GetExperienceByIdAsync(id);
            if (result.Result == null)
            {
                return NotFound(result.ValidationMessage ?? "Experiența nu a fost găsită.");
            }

            return Ok(result.Result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetExperienceByCityOrCoords([FromQuery] string? city, [FromQuery] double? lat, [FromQuery] double? lon)
        {
            var result = await _experienceService.GetExperienceByCityOrCoords(city, lat, lon);
            if (result.Result == null || !result.Result.Any())
            {
                return NotFound(result.ValidationMessage ?? "Nu s-au găsit experiențe pentru criteriile specificate.");
            }

            return Ok(result.Result);
        }

        [HttpPost]
        public async Task<IActionResult> AddExperience([FromBody] ExperienceModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datele experienței sunt invalide.");
            }

            var result = await _experienceService.AddExperience(request);
            if (result.Result == null)
            {
                return BadRequest("Experiența nu a putut fi adăugată.");
            }

            return CreatedAtAction(nameof(GetExperienceById), new { id = result.Result.Id }, result.Result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExperience([FromBody] ExperienceModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datele experienței sunt invalide.");
            }

            var result = await _experienceService.UpdateExperience(request);
            if (result.Result == null)
            {
                return NotFound(result.ValidationMessage ?? "Experiența nu a putut fi actualizată.");
            }

            return Ok(result.Result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var result = await _experienceService.DeleteExperience(id);
            if (!result.Result)
            {
                return NotFound(result.ValidationMessage ?? "Experiența nu a putut fi ștearsă.");
            }

            return NoContent();
        }
    }
}
