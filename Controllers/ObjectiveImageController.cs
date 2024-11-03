using Microsoft.AspNetCore.Mvc;
using TravelSBE.Services;

[Route("api/[controller]")]
[ApiController]
public class ObjectiveImageController : ControllerBase
{
    private readonly IObjectiveImageService _imageService;

    public ObjectiveImageController(IObjectiveImageService imageService)
    {
        _imageService = imageService;
    }

    // GET: api/ObjectiveImage/{objectiveId}
    [HttpGet("{objectiveId}")]
    public async Task<IActionResult> GetImages(int objectiveId)
    {
        var images = await _imageService.GetImagesByObjectiveIdAsync(objectiveId);
        return Ok(images.Select(img => new
        {
            img.Id,
            img.FilePath,
            img.ImageMimeType
        }));
    }

    // POST: api/ObjectiveImage/{objectiveId}/upload
    [HttpPost("{objectiveId}/upload")]
    public async Task<IActionResult> UploadImage(int objectiveId, [FromForm] IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("Invalid image file.");

        var result = await _imageService.UploadImageAsync(imageFile, objectiveId);
        if (!result.IsSuccessful)
            return BadRequest(result.ValidationMessage);

        return Ok(new { ImageId = result.Result });
    }

    // DELETE: api/ObjectiveImage/delete/{imageId}
    [HttpDelete("delete/{imageId}")]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var result = await _imageService.DeleteImageAsync(imageId);
        if (!result.IsSuccessful)
            return BadRequest(result.ValidationMessage);

        return Ok("Image deleted successfully.");
    }
}
