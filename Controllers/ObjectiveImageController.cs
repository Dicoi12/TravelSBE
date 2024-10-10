using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        [HttpPost]
        [Route("addimage")]
        public async Task<IActionResult> AddImage([FromForm] int objectiveId, [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Invalid image file.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                var imageModel = new ObjectiveImageModel
                {
                    IdObjective = objectiveId,
                    ImageData = imageData,
                    ImageMimeType = imageFile.ContentType
                };

                var result = await _objectiveImageService.AddImageAsync(imageModel);

                if (!string.IsNullOrEmpty(result.ValidationMessage))
                {
                    return BadRequest(result.ValidationMessage);
                }

                return Ok(result.Result);
            }
        }

        [HttpGet]
        [Route("getimage/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var result = await _objectiveImageService.GetImageByIdAsync(id);

            if (result.Result == null || !string.IsNullOrEmpty(result.ValidationMessage))
            {
                return NotFound(result.ValidationMessage ?? "Image not found.");
            }
            return File(result.Result.ImageData, result.Result.ImageMimeType);
        }

        [HttpGet]
        [Route("getimagesbyobjective/{objectiveId}")]
        public async Task<IActionResult> GetImagesByObjective(int objectiveId)
        {
            var result = await _objectiveImageService.GetImagesByObjectiveIdAsync(objectiveId);

            if (result.Result == null || result.Result.Count == 0)
            {
                return NotFound("No images found for this objective.");
            }

            return Ok(result.Result);
        }

        [HttpDelete]
        [Route("deleteimage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _objectiveImageService.DeleteImageAsync(id);

            if (!result.Result)
            {
                return NotFound(result.ValidationMessage ?? "Image not found.");
            }

            return Ok("Image deleted successfully.");
        }
    }
}
