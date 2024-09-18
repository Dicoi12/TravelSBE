using Microsoft.AspNetCore.Mvc;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectiveImageController : ControllerBase
    {
        private readonly IObjectiveImageService _objectiveImageService;

        public ObjectiveImageController(IObjectiveImageService objectiveImageService)
        {
            _objectiveImageService = objectiveImageService;
        }

        // Obține o imagine după ID
        [HttpGet("{id}")]
        public async Task<ServiceResult<ObjectiveImageModel>> GetImageById(int id)
        {
            var result = await _objectiveImageService.GetImageByIdAsync(id);
            return result;
        }

        // Obține toate imaginile pentru un obiectiv specific
        [HttpGet("objective/{objectiveId}")]
        public async Task<ServiceResult<List<ObjectiveImageModel>>> GetImagesByObjectiveId(int objectiveId)
        {
            var result = await _objectiveImageService.GetImagesByObjectiveIdAsync(objectiveId);
            return result;
        }

        // Adaugă o imagine pentru un obiectiv
        [HttpPost]
        public async Task<ServiceResult<ObjectiveImageModel>> AddImage([FromBody] ObjectiveImageModel imageModel)
        {
            var result = await _objectiveImageService.AddImageAsync(imageModel);
            return result;
        }

        // Șterge o imagine după ID
        [HttpDelete("{id}")]
        public async Task<ServiceResult<bool>> DeleteImage(int id)
        {
            var result = await _objectiveImageService.DeleteImageAsync(id);
            return result;
        }
    }
}
